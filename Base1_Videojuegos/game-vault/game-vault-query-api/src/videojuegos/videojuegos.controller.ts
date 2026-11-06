import {
  Controller,
  Get,
  Post,
  Body,
  Patch,
  Param,
  Delete,
  UseGuards,
} from '@nestjs/common';
import { VideojuegosService } from './videojuegos.service';
import { CreateVideojuegoDto } from './dto/create-videojuego.dto';
import { UpdateVideojuegoDto } from './dto/update-videojuego.dto';
import { AuthGuard } from '../auth/auth.guard';

@Controller('videojuegos')
export class VideojuegosController {
  constructor(private readonly videojuegosService: VideojuegosService) {}

  @Post()
  @UseGuards(AuthGuard)
  create(@Body() createVideojuegoDto: CreateVideojuegoDto) {
    return this.videojuegosService.create(createVideojuegoDto);
  }

  @Get()
  findAll() {
    return this.videojuegosService.findAll();
  }

  @Get(':id')
  findOne(@Param('id') id: string) {
    return this.videojuegosService.findOne(+id);
  }

  @Patch(':id')
  @UseGuards(AuthGuard)
  update(@Param('id') id: string, @Body() updateVideojuegoDto: UpdateVideojuegoDto) {
    return this.videojuegosService.update(+id, updateVideojuegoDto);
  }

  @Delete(':id')
  @UseGuards(AuthGuard)
  remove(@Param('id') id: string) {
    return this.videojuegosService.remove(+id);
  }
}