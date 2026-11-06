import { describe, it, expect, beforeEach, jest } from '@jest/globals';
import { Test, TestingModule } from '@nestjs/testing';
import { ConflictException, UnauthorizedException } from '@nestjs/common';
import { JwtModule } from '@nestjs/jwt';

jest.unstable_mockModule('bcryptjs', () => ({
  hash: jest.fn(),
  compare: jest.fn(),
}));

const { AuthService } = await import('./auth.service');
const { UsersService } = await import('../users/users.service');
const { PrismaService } = await import('../prisma.service');

type BcryptMock = { hash: jest.Mock; compare: jest.Mock };

describe('AuthService', () => {
  let service: InstanceType<typeof AuthService>;
  let bcrypt: BcryptMock;
  let prisma: {
    user: {
      create: jest.Mock;
      findUnique: jest.Mock;
    };
  };

  beforeEach(async () => {
    prisma = {
      user: {
        create: jest.fn(),
        findUnique: jest.fn(),
      },
    };
    bcrypt = (await import('bcryptjs')) as unknown as BcryptMock;
    bcrypt.hash.mockResolvedValue('hashed-password');
    bcrypt.compare.mockResolvedValue(true);

    const module: TestingModule = await Test.createTestingModule({
      imports: [JwtModule.register({ secret: 'test-secret' })],
      providers: [
        AuthService,
        UsersService,
        { provide: PrismaService, useValue: prisma },
      ],
    }).compile();

    service = module.get(AuthService);
  });

  it('should be defined', () => {
    expect(service).toBeDefined();
  });

  describe('register', () => {
    it('should create a user and return it without the password', async () => {
      prisma.user.create.mockResolvedValue({
        id: 1,
        email: 'user@example.com',
        password: 'hashed-password',
      });

      await expect(service.register('user@example.com', 'plain-password')).resolves.toEqual({
        id: 1,
        email: 'user@example.com',
      });
      expect(bcrypt.hash).toHaveBeenCalledWith('plain-password', 10);
      expect(prisma.user.create).toHaveBeenCalledWith({
        data: { email: 'user@example.com', password: 'hashed-password' },
      });
    });

    it('should throw ConflictException when the email is already registered', async () => {
      prisma.user.create.mockRejectedValue({ code: 'P2002' });

      await expect(service.register('user@example.com', 'plain-password')).rejects.toThrow(
        ConflictException,
      );
    });
  });

  describe('login', () => {
    it('should return an access token for valid credentials', async () => {
      prisma.user.findUnique.mockResolvedValue({
        id: 1,
        email: 'user@example.com',
        password: 'hashed-password',
      });
      bcrypt.compare.mockResolvedValue(true);

      const result = await service.login('user@example.com', 'plain-password');

      expect(result).toHaveProperty('access_token');
      expect(typeof result.access_token).toBe('string');
      expect(bcrypt.compare).toHaveBeenCalledWith('plain-password', 'hashed-password');
    });

    it('should throw UnauthorizedException when the user does not exist', async () => {
      prisma.user.findUnique.mockResolvedValue(null);

      await expect(service.login('user@example.com', 'plain-password')).rejects.toThrow(
        UnauthorizedException,
      );
    });

    it('should throw UnauthorizedException when the password is wrong', async () => {
      prisma.user.findUnique.mockResolvedValue({
        id: 1,
        email: 'user@example.com',
        password: 'hashed-password',
      });
      bcrypt.compare.mockResolvedValue(false);

      await expect(service.login('user@example.com', 'wrong-password')).rejects.toThrow(
        UnauthorizedException,
      );
    });
  });
});