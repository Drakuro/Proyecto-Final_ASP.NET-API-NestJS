import { Injectable, NotFoundException } from '@nestjs/common';
import { PrismaService } from '../prisma.service';
import { CreateVideojuegoDto } from './dto/create-videojuego.dto';
import { UpdateVideojuegoDto } from './dto/update-videojuego.dto';

@Injectable()
export class VideojuegosService {
  constructor(private readonly prisma: PrismaService) {}

  create(createVideojuegoDto: CreateVideojuegoDto) {
    return this.prisma.videojuego.create({ data: createVideojuegoDto });
  }

  findAll() {
    return this.prisma.videojuego.findMany();
  }

  async findOne(id: number) {
    const videojuego = await this.prisma.videojuego.findUnique({ where: { id } });
    if (!videojuego) {
      throw new NotFoundException(`Videojuego with id ${id} not found`);
    }
    return videojuego;
  }

  async update(id: number, updateVideojuegoDto: UpdateVideojuegoDto) {
    await this.findOne(id);
    return this.prisma.videojuego.update({
      where: { id },
      data: updateVideojuegoDto,
    });
  }

  async remove(id: number) {
    await this.findOne(id);
    return this.prisma.videojuego.delete({ where: { id } });
  }
}