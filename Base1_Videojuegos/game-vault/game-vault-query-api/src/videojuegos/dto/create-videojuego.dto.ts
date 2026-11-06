import { IsString } from 'class-validator';

export class CreateVideojuegoDto {
  @IsString()
  nombreDelJuego: string;

  @IsString()
  imagenDelJuego: string;
}