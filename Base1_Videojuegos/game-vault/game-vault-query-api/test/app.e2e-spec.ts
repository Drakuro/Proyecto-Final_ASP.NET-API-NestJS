import { Test, TestingModule } from '@nestjs/testing';
import { INestApplication, ValidationPipe } from '@nestjs/common';
import request from 'supertest';
import { App } from 'supertest/types';
import { AppModule } from './../src/app.module';

describe('VideojuegosController (e2e)', () => {
  let app: INestApplication<App>;

  beforeEach(async () => {
    const moduleFixture: TestingModule = await Test.createTestingModule({
      imports: [AppModule],
    }).compile();

    app = moduleFixture.createNestApplication();
    app.useGlobalPipes(new ValidationPipe({ whitelist: true, transform: true }));
    await app.init();
  });

  it('/videojuegos (GET)', () => {
    return request(app.getHttpServer()).get('/videojuegos').expect(200);
  });

  it('POST /videojuegos without token returns 401', () => {
    return request(app.getHttpServer())
      .post('/videojuegos')
      .send({ nombreDelJuego: 'Halo', imagenDelJuego: 'halo.png' })
      .expect(401);
  });

  it('auth flow: register, login, then create a videojuego with the token', async () => {
    const email = `e2e-${Date.now()}@test.com`;
    const password = 'supersecret123';

    await request(app.getHttpServer())
      .post('/auth/register')
      .send({ email, password })
      .expect(201);

    const loginResponse = await request(app.getHttpServer())
      .post('/auth/login')
      .send({ email, password })
      .expect(200);

    const accessToken = loginResponse.body.access_token;
    expect(typeof accessToken).toBe('string');

    return request(app.getHttpServer())
      .post('/videojuegos')
      .set('Authorization', `Bearer ${accessToken}`)
      .send({ nombreDelJuego: 'Halo 3', imagenDelJuego: 'halo3.png' })
      .expect(201);
  });

  afterEach(async () => {
    await app.close();
  });
});