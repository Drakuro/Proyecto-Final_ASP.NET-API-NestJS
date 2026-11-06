import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
import VideojuegosView from '../views/VideojuegosView.vue';

const router = createRouter({
	history: createWebHistory(),
	routes: [
		{ path: '/', redirect: '/videojuegos' },
		{ path: '/login', component: LoginView },
		{ path: '/registro', component: RegisterView },
		{
			path: '/videojuegos',
			component: VideojuegosView,
			meta: { requiresAuth: true },
		},
	],
});

// El guard se ejecuta antes de cambiar de ruta.
router.beforeEach((to) => {
	const token = localStorage.getItem('access_token');

	if (to.meta.requiresAuth && !token) {
		return '/login';
	}

	return true;
});

export default router;
