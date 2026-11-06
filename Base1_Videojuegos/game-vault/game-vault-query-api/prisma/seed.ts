import 'dotenv/config';
import { PrismaClient } from '../src/generated/prisma/client.js';
import { PrismaBetterSqlite3 } from '@prisma/adapter-better-sqlite3';

interface VideojuegoSeed {
  nombreDelJuego: string;
  imagenDelJuego: string;
}

const adapter = new PrismaBetterSqlite3({ url: process.env.DATABASE_URL as string });
const prisma = new PrismaClient({ adapter });

const juegos: VideojuegoSeed[] = [
  { nombreDelJuego: "PUBG: Battlegrounds", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/578080/header.jpg" },
  { nombreDelJuego: "Counter-Strike 2", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/730/header.jpg" },
  { nombreDelJuego: "Dota 2", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/570/header.jpg" },
  { nombreDelJuego: "Grand Theft Auto V", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg" },
  { nombreDelJuego: "Elden Ring", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/header.jpg" },
  { nombreDelJuego: "Baldur's Gate 3", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/1086940/header.jpg" },
  { nombreDelJuego: "Cyberpunk 2077", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg" },
  { nombreDelJuego: "Red Dead Redemption 2", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg" },
  { nombreDelJuego: "The Witcher 3: Wild Hunt", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg" },
  { nombreDelJuego: "God of War", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/1593500/header.jpg" },
  { nombreDelJuego: "Sekiro: Shadows Die Twice", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/814380/header.jpg" },
  { nombreDelJuego: "Hogwarts Legacy", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/990080/header.jpg" },
  { nombreDelJuego: "Resident Evil 4", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/2050650/header.jpg" },
  { nombreDelJuego: "Monster Hunter: World", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/582010/header.jpg" },
  { nombreDelJuego: "Stardew Valley", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg" },
  { nombreDelJuego: "Hollow Knight", imagenDelJuego: "https://cdn.cloudflare.steamstatic.com/steam/apps/367520/header.jpg" }
];

async function main(): Promise<void> {
  const existing = await prisma.videojuego.count();
  if (existing > 0) {
    console.log(`La tabla ya tiene ${existing} registros. Omitiendo seed.`);
    return;
  }
  const { count } = await prisma.videojuego.createMany({ data: juegos });
  console.log(`Insertados ${count} videojuegos.`);
  const all = await prisma.videojuego.findMany();
  console.log(`Total en tabla: ${all.length}`);
  all.forEach((j) => console.log(`${j.id}\t${j.nombreDelJuego}\t${j.imagenDelJuego}`));
}

main()
  .catch((e) => {
    console.error(e);
    process.exitCode = 1;
  })
  .finally(() => prisma.$disconnect());