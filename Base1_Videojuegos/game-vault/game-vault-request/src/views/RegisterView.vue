<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:3000';
const email = ref('');
const password = ref('');
const error = ref('');
const cargando = ref(false);

async function registrarse() {
	error.value = '';
	cargando.value = true;

	try {
		const respuesta = await fetch(`${API_BASE_URL}/auth/register`, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify({ email: email.value, password: password.value }),
		});

		if (!respuesta.ok) {
			throw new Error('No se pudo crear la cuenta');
		}

		await router.push('/login');
	} catch (e) {
		error.value = e instanceof Error ? e.message : 'No se pudo registrar';
	} finally {
		cargando.value = false;
	}
}
</script>

<template>
	<main>
		<h1>Crear cuenta</h1>
		<form @submit.prevent="registrarse">
			<label for="email">Correo</label>
			<input id="email" v-model="email" type="email" required />

			<label for="password">Contraseña</label>
			<input
				id="password"
				v-model="password"
				type="password"
				minlength="8"
				required
			/>

			<button type="submit" :disabled="cargando">
				{{ cargando ? 'Registrando...' : 'Registrarse' }}
			</button>
		</form>

		<p v-if="error">{{ error }}</p>
		<RouterLink to="/login">Ya tengo una cuenta</RouterLink>
	</main>
</template>