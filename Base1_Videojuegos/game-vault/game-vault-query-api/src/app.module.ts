import { Module } from '@nestjs/common';
import { PrismaModule } from './prisma.module';
import { VideojuegosModule } from './videojuegos/videojuegos.module';
import { AuthModule } from './auth/auth.module';

@Module({
  imports: [PrismaModule, VideojuegosModule, AuthModule],
})
export class AppModule {}