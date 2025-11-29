<template>
  <div>
    <h2 class="text-3xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-indigo-600 to-violet-600 mb-8">
      Carrito de Compras
    </h2>
    
    <!-- Empty State -->
    <div v-if="cartStore.items.length === 0" class="glass-card p-12 text-center">
      <div class="max-w-md mx-auto">
        <div class="w-24 h-24 mx-auto mb-6 rounded-full bg-gradient-to-tr from-indigo-100 to-violet-100 flex items-center justify-center">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-indigo-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
          </svg>
        </div>
        <h3 class="text-2xl font-bold text-slate-700 mb-3">Tu carrito está vacío</h3>
        <p class="text-slate-500 mb-6">Agrega productos desde el catálogo para comenzar tu compra</p>
        <router-link to="/catalog" class="btn-primary inline-block">
          Explorar Productos
        </router-link>
      </div>
    </div>
    
    <!-- Cart Items -->
    <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Items List -->
      <div class="lg:col-span-2 space-y-4">
        <div
          v-for="item in cartStore.items"
          :key="item.id"
          class="glass-card p-6 hover:shadow-xl transition-all duration-300"
        >
          <div class="flex items-center gap-6">
            <!-- Product Image -->
            <div class="w-24 h-24 flex-shrink-0 rounded-xl bg-gradient-to-br from-indigo-100 to-purple-100 flex items-center justify-center">
              <span class="text-3xl">📦</span>
            </div>
            
            <!-- Product Details -->
            <div class="flex-1 min-w-0">
              <h3 class="text-lg font-bold text-slate-800 mb-1 truncate">
                {{ item.name }}
              </h3>
              <p class="text-xl font-bold text-indigo-600">
                ${{ item.price.toLocaleString() }}
              </p>
              <p class="text-sm text-slate-500 mt-1">
                Subtotal: ${{ (item.price * item.quantity).toLocaleString() }}
              </p>
            </div>
            
            <!-- Quantity Controls -->
            <div class="flex items-center gap-3">
              <div class="flex items-center bg-white/50 rounded-xl border border-slate-200 overflow-hidden">
                <button
                  @click="cartStore.updateQuantity(item.id, item.quantity - 1)"
                  class="px-4 py-2 text-slate-600 hover:bg-indigo-50 hover:text-indigo-600 transition-colors"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 12H4" />
                  </svg>
                </button>
                <span class="px-4 py-2 font-bold text-slate-700 min-w-[3rem] text-center">
                  {{ item.quantity }}
                </span>
                <button
                  @click="cartStore.updateQuantity(item.id, item.quantity + 1)"
                  class="px-4 py-2 text-slate-600 hover:bg-indigo-50 hover:text-indigo-600 transition-colors"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                  </svg>
                </button>
              </div>
              
              <button
                @click="cartStore.removeFromCart(item.id)"
                class="p-3 text-red-400 hover:text-red-600 hover:bg-red-50 rounded-xl transition-all duration-300"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Order Summary -->
      <div class="lg:col-span-1">
        <div class="glass-card p-6 sticky top-24 space-y-6">
          <h3 class="text-xl font-bold text-slate-800 border-b border-slate-200 pb-3">
            Resumen de Orden
          </h3>
          
          <div class="space-y-3">
            <div class="flex justify-between text-slate-600">
              <span>Productos ({{ cartStore.totalItems }})</span>
              <span class="font-medium">${{ cartStore.subtotal.toLocaleString() }}</span>
            </div>
            
            <div class="flex justify-between text-slate-600">
              <span>IVA (19%)</span>
              <span class="font-medium">${{ cartStore.tax.toLocaleString() }}</span>
            </div>
            
            <div class="border-t border-slate-200 pt-3 flex justify-between text-lg font-bold text-slate-800">
              <span>Total</span>
              <span class="text-2xl text-indigo-600">${{ cartStore.total.toLocaleString() }}</span>
            </div>
          </div>
          
          <div v-if="error" class="rounded-xl bg-red-50 border border-red-200 p-4">
            <p class="text-sm text-red-600 text-center">{{ error }}</p>
          </div>
          
          <button
            @click="handleCheckout"
            :disabled="loading"
            class="w-full py-4 px-6 bg-gradient-to-r from-indigo-600 to-violet-600 text-white font-bold rounded-xl hover:from-indigo-700 hover:to-violet-700 transition-all duration-300 shadow-lg shadow-indigo-500/30 hover:shadow-indigo-500/50 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="loading" class="flex items-center justify-center">
              <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              Procesando...
            </span>
            <span v-else class="flex items-center justify-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
              </svg>
              Confirmar Compra
            </span>
          </button>
          
          <p class="text-xs text-center text-slate-500">
            Al confirmar, recibirás un correo con los detalles de tu compra
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useCartStore } from '../stores/cart';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';

const cartStore = useCartStore();
const authStore = useAuthStore();
const router = useRouter();
const loading = ref(false);
const error = ref('');

const handleCheckout = async () => {
  console.log('User object:', authStore.user);
  
  if (!authStore.user) {
    error.value = 'Usuario no identificado. Por favor inicie sesión nuevamente.';
    return;
  }

  // El backend espera userName que es el email del usuario
  const userEmail = authStore.user.email || authStore.user.Email;
  
  if (!userEmail) {
    error.value = 'No se pudo obtener el email del usuario.';
    console.error('User object:', authStore.user);
    return;
  }

  loading.value = true;
  error.value = '';
  
  try {
    console.log('Attempting checkout with email:', userEmail);
    await cartStore.checkout(userEmail);
    alert('¡Compra realizada con éxito! Se ha enviado un correo de confirmación.');
    router.push('/catalog');
  } catch (err) {
    console.error('Checkout error:', err);
    console.error('Error response:', err.response?.data);
    
    // Mostrar mensaje de error más específico
    if (err.response?.data?.message) {
      error.value = err.response.data.message;
    } else if (err.response?.data) {
      error.value = JSON.stringify(err.response.data);
    } else {
      error.value = 'Error al procesar la compra. Intente nuevamente.';
    }
  } finally {
    loading.value = false;
  }
};
</script>
