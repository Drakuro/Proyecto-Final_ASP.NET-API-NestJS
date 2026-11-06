import { Module } from '@nestjs/common';
import { VideojuegosService } from './videojuegos.service';
import { VideojuegosController } from './videojuegos.controller';
import { AuthGuard } from '../auth/auth.guard';

@Module({
  controllers: [VideojuegosController],
  providers: [VideojuegosService, AuthGuard],
})
export class VideojuegosModule {}