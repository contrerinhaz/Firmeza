<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-50 via-purple-50 to-pink-50 py-12 px-4 sm:px-6 lg:px-8">
    <div class="max-w-md w-full space-y-8">
      <!-- Logo/Header -->
      <div class="text-center">
        <div class="mx-auto h-16 w-16 bg-gradient-to-tr from-indigo-600 to-violet-600 rounded-2xl flex items-center justify-center text-white font-bold text-3xl shadow-2xl shadow-indigo-500/30">
          F
        </div>
        <h2 class="mt-6 text-4xl font-extrabold bg-clip-text text-transparent bg-gradient-to-r from-indigo-600 to-violet-600">
          Únete a Firmeza
        </h2>
        <p class="mt-2 text-sm text-slate-600">
          Crea tu cuenta y comienza a comprar
        </p>
      </div>

      <!-- Register Form -->
      <div class="glass-card p-8 space-y-6">
        <form @submit.prevent="handleRegister" class="space-y-4">
          <div>
            <label for="fullName" class="block text-sm font-medium text-slate-700 mb-2">
              Nombre Completo
            </label>
            <input
              id="fullName"
              v-model="form.fullName"
              type="text"
              required
              class="input-field"
              placeholder="Juan Pérez"
            />
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label for="documentNumber" class="block text-sm font-medium text-slate-700 mb-2">
                Documento
              </label>
              <input
                id="documentNumber"
                v-model="form.documentNumber"
                type="text"
                required
                class="input-field"
                placeholder="12345678"
              />
            </div>
            <div>
              <label for="phone" class="block text-sm font-medium text-slate-700 mb-2">
                Teléfono
              </label>
              <input
                id="phone"
                v-model="form.phone"
                type="text"
                required
                class="input-field"
                placeholder="123456789"
              />
            </div>
          </div>

          <div>
            <label for="email" class="block text-sm font-medium text-slate-700 mb-2">
              Correo Electrónico
            </label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              class="input-field"
              placeholder="tu@email.com"
            />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-slate-700 mb-2">
              Contraseña
            </label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              class="input-field"
              placeholder="Mínimo 6 caracteres"
            />
          </div>

          <div v-if="error" class="rounded-xl bg-red-50 border border-red-200 p-4">
            <p class="text-sm text-red-600 text-center">{{ error }}</p>
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="btn-primary w-full disabled:opacity-50 disabled:cursor-not-allowed mt-6"
          >
            <span v-if="loading" class="flex items-center justify-center">
              <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Registrando...
            </span>
            <span v-else>Crear Cuenta</span>
          </button>
        </form>

        <div class="relative">
          <div class="absolute inset-0 flex items-center">
            <div class="w-full border-t border-slate-200"></div>
          </div>
          <div class="relative flex justify-center text-sm">
            <span class="px-4 bg-white/80 text-slate-500">¿Ya tienes cuenta?</span>
          </div>
        </div>

        <router-link
          to="/login"
          class="block w-full text-center py-3 px-6 border-2 border-indigo-200 text-indigo-600 font-semibold rounded-xl hover:bg-indigo-50 hover:border-indigo-300 transition-all duration-300"
        >
          Iniciar Sesión
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';

const form = reactive({
  fullName: '',
  documentNumber: '',
  phone: '',
  email: '',
  password: ''
});

const error = ref('');
const loading = ref(false);
const authStore = useAuthStore();
const router = useRouter();

const handleRegister = async () => {
  loading.value = true;
  error.value = '';
  try {
    await authStore.register(form);
    await authStore.login({ email: form.email, password: form.password });
    router.push('/catalog');
  } catch (err) {
    error.value = 'Error en el registro. Verifique los datos.';
  } finally {
    loading.value = false;
  }
};
</script>
