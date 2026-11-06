<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';

interface Videojuego {
	id: number;
	nombreDelJuego: string;
	imagenDelJuego: string;
}

const router = useRouter();
const videojuegos = ref<Videojuego[]>([]);
const textoBusqueda = ref('');
const cargando = ref(true);
const error = ref('');
const paginaActual = ref(1);
const elementosPorPagina = 4;
const nombreDelJuego = ref('');
const imagenDelJuego = ref('');
const videojuegoEditandoId = ref<number | null>(null);
const mostrarFormulario = ref(false);
const guardando = ref(false);

const videojuegosFiltrados = computed(() => {
	const texto = textoBusqueda.value.trim().toLowerCase();
	return videojuegos.value.filter((videojuego) =>
		videojuego.nombreDelJuego.toLowerCase().includes(texto),
	);
});

const totalPaginas = computed(() =>
	Math.ceil(videojuegosFiltrados.value.length / elementosPorPagina),
);

const videojuegosPaginados = computed(() => {
	const inicio = (paginaActual.value - 1) * elementosPorPagina;
	return videojuegosFiltrados.value.slice(inicio, inicio + elementosPorPagina);
});

async function cargarVideojuegos() {
	try {
		const respuesta = await fetch('http://localhost:3000/videojuegos');
		if (!respuesta.ok) throw new Error('No se pudieron cargar los videojuegos');
		videojuegos.value = await respuesta.json();
	} catch (e) {
		error.value = e instanceof Error ? e.message : 'Error al cargar videojuegos';
	} finally {
		cargando.value = false;
	}
}

function obtenerHeadersAutenticados() {
	const token = localStorage.getItem('access_token');

	return {
		'Content-Type': 'application/json',
		Authorization: `Bearer ${token}`,
	};
}

async function guardarVideojuego() {
	error.value = '';
	guardando.value = true;

	const editando = videojuegoEditandoId.value !== null;
	const url = editando
		? `http://localhost:3000/videojuegos/${videojuegoEditandoId.value}`
		: 'http://localhost:3000/videojuegos';

	try {
		const respuesta = await fetch(url, {
			method: editando ? 'PATCH' : 'POST',
			headers: obtenerHeadersAutenticados(),
			body: JSON.stringify({
				nombreDelJuego: nombreDelJuego.value,
				imagenDelJuego: imagenDelJuego.value,
			}),
		});

		if (!respuesta.ok) {
			if (respuesta.status === 401) {
				cerrarSesion();
				return;
			}
			throw new Error('No se pudo guardar el videojuego');
		}

		limpiarFormulario();
		await cargarVideojuegos();
	} catch (e) {
		error.value = e instanceof Error ? e.message : 'Error al guardar';
	} finally {
		guardando.value = false;
	}
}

function prepararEdicion(videojuego: Videojuego) {
	videojuegoEditandoId.value = videojuego.id;
	nombreDelJuego.value = videojuego.nombreDelJuego;
	imagenDelJuego.value = videojuego.imagenDelJuego;
	mostrarFormulario.value = true;
}

function prepararCreacion() {
	limpiarFormulario();
	mostrarFormulario.value = true;
}

function limpiarFormulario() {
	videojuegoEditandoId.value = null;
	nombreDelJuego.value = '';
	imagenDelJuego.value = '';
	mostrarFormulario.value = false;
}

async function eliminarVideojuego(id: number) {
	if (!window.confirm('¿Deseas eliminar este videojuego?')) return;

	try {
		const respuesta = await fetch(`http://localhost:3000/videojuegos/${id}`, {
			method: 'DELETE',
			headers: obtenerHeadersAutenticados(),
		});

		if (!respuesta.ok) {
			if (respuesta.status === 401) {
				cerrarSesion();
				return;
			}
			throw new Error('No se pudo eliminar el videojuego');
		}

		await cargarVideojuegos();
		if (paginaActual.value > totalPaginas.value) {
			paginaActual.value = Math.max(totalPaginas.value, 1);
		}
	} catch (e) {
		error.value = e instanceof Error ? e.message : 'Error al eliminar';
	}
}

function paginaAnterior() {
	if (paginaActual.value > 1) paginaActual.value--;
}

function paginaSiguiente() {
	if (paginaActual.value < totalPaginas.value) paginaActual.value++;
}

function cerrarSesion() {
	localStorage.removeItem('access_token');
	router.push('/login');
}

watch(textoBusqueda, () => {
	paginaActual.value = 1;
});

onMounted(cargarVideojuegos);
</script>

<template>
	<main>
		<button type="button" @click="cerrarSesion">Cerrar sesión</button>
		<h1>Game Vault</h1>

		<button type="button" @click="prepararCreacion">Agregar videojuego</button>

		<form v-if="mostrarFormulario" @submit.prevent="guardarVideojuego">
			<h2>{{ videojuegoEditandoId === null ? 'Agregar videojuego' : 'Editar videojuego' }}</h2>
			<label for="nombreDelJuego">Nombre</label>
			<input id="nombreDelJuego" v-model="nombreDelJuego" type="text" required />

			<label for="imagenDelJuego">URL de imagen</label>
			<input id="imagenDelJuego" v-model="imagenDelJuego" type="url" required />

			<button type="submit" :disabled="guardando">
				{{ guardando ? 'Guardando...' : videojuegoEditandoId === null ? 'Crear' : 'Actualizar' }}
			</button>
			<button v-if="videojuegoEditandoId !== null" type="button" @click="limpiarFormulario">
				Cancelar edición
			</button>
		</form>

		<p v-if="cargando">Cargando videojuegos...</p>
		<p v-else-if="error">{{ error }}</p>

		<section v-else>
			<label for="busqueda">Buscar por nombre</label>
			<input id="busqueda" v-model="textoBusqueda" type="search" />
			<p v-if="videojuegosFiltrados.length === 0">No se encontraron videojuegos.</p>

			<article v-for="videojuego in videojuegosPaginados" :key="videojuego.id">
				<img :src="videojuego.imagenDelJuego" :alt="videojuego.nombreDelJuego" />
				<h2>{{ videojuego.nombreDelJuego }}</h2>
				<button type="button" @click="prepararEdicion(videojuego)">Editar</button>
				<button type="button" @click="eliminarVideojuego(videojuego.id)">Eliminar</button>
			</article>

			<nav v-if="totalPaginas > 1" aria-label="Paginación">
				<button type="button" :disabled="paginaActual === 1" @click="paginaAnterior">
					Anterior
				</button>
				<span>Página {{ paginaActual }} de {{ totalPaginas }}</span>
				<button
					type="button"
					:disabled="paginaActual === totalPaginas"
					@click="paginaSiguiente"
				>
					Siguiente
				</button>
			</nav>
		</section>
	</main>
</template>
