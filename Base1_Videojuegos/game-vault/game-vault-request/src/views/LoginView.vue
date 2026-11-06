<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const email = ref('');
const password = ref('');
const error = ref('');
const cargando = ref(false);

async function iniciarSesion() {
	error.value = '';
	cargando.value = true;

	try {
		const respuesta = await fetch('http://localhost:3000/auth/login', {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email: email.value, password: password.value }),
		});

		if (!respuesta.ok) {
			throw new Error('Correo o contraseña incorrectos');
		}

        // Persistencia del token de acceso en el almacenamiento local
		const datos: { access_token: string } = await respuesta.json();
		localStorage.setItem('access_token', datos.access_token);
		await router.push('/videojuegos');
	} catch (e) {
		error.value = e instanceof Error ? e.message : 'No se pudo iniciar sesión';
	} finally {
		cargando.value = false;
	}
}
</script>

<template>
	<main>
		<h1>Iniciar sesión</h1>
		<form @submit.prevent="iniciarSesion">
			<label for="email">Correo</label>
			<input id="email" v-model="email" type="email" required />

			<label for="password">Contraseña</label>
			<input id="password" v-model="password" type="password" required />

			<button type="submit" :disabled="cargando">
				{{ cargando ? 'Ingresando...' : 'Ingresar' }}
			</button>
		</form>

		<p v-if="error">{{ error }}</p>
		<RouterLink to="/registro">Crear una cuenta</RouterLink>
	</main>
</template>
