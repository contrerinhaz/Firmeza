import { defineStore } from 'pinia';
import api from '../services/api';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        token: localStorage.getItem('token') || null,
        user: JSON.parse(localStorage.getItem('user')) || null
    }),
    getters: {
        isAuthenticated: (state) => !!state.token
    },
    actions: {
        async login(credentials) {
            try {
                const response = await api.post('/Auth/login', credentials);

                // Verificar que el usuario sea Cliente y NO Admin
                const userRoles = response.data.user.roles || [];

                if (userRoles.includes('Admin')) {
                    throw new Error('Los administradores no tienen acceso a esta aplicación. Por favor use el sistema de administración.');
                }

                if (!userRoles.includes('Cliente')) {
                    throw new Error('Solo los clientes pueden acceder a esta aplicación.');
                }

                this.token = response.data.token;
                this.user = response.data.user;

                localStorage.setItem('token', this.token);
                localStorage.setItem('user', JSON.stringify(this.user));

                return true;
            } catch (error) {
                console.error('Login failed', error);
                throw error;
            }
        },
        async register(userData) {
            try {
                await api.post('/Auth/register', userData);
                return true;
            } catch (error) {
                console.error('Registration failed', error);
                throw error;
            }
        },
        logout() {
            this.token = null;
            this.user = null;
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
    }
});
