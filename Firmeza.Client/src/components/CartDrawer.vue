<template>
  <!-- Backdrop -->
  <transition name="fade">
    <div 
      v-if="isOpen" 
      class="fixed inset-0 bg-black/50 backdrop-blur-sm z-40"
      @click="close"
    ></div>
  </transition>

  <!-- Drawer -->
  <transition name="slide">
    <div 
      v-if="isOpen" 
      class="fixed right-0 top-0 h-full w-full sm:w-[480px] bg-white shadow-2xl z-50 flex flex-col"
    >
      <!-- Header -->
      <div class="flex items-center justify-between p-6 border-b border-slate-200 bg-gradient-to-r from-indigo-50 to-violet-50">
        <h2 class="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-indigo-600 to-violet-600">
          Carrito de Compras
        </h2>
        <button 
          @click="close" 
          class="p-2 text-slate-400 hover:text-slate-600 hover:bg-white rounded-xl transition-all duration-200"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Empty State -->
      <div v-if="cartStore.items.length === 0" class="flex-1 flex items-center justify-center p-8">
        <div class="text-center">
          <div class="w-24 h-24 mx-auto mb-6 rounded-full bg-gradient-to-tr from-indigo-100 to-violet-100 flex items-center justify-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-indigo-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
          </div>
          <h3 class="text-xl font-bold text-slate-700 mb-2">Tu carrito está vacío</h3>
          <p class="text-slate-500 text-sm mb-4">Agrega productos desde el catálogo</p>
          <button @click="close" class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors">
            Explorar Productos
          </button>
        </div>
      </div>

      <!-- Cart Items -->
      <div v-else class="flex-1 overflow-y-auto p-6 space-y-4 custom-scrollbar">
        <div
          v-for="item in cartStore.items"
          :key="item.id"
          class="p-4 bg-white border border-slate-200 rounded-xl hover:border-indigo-300 hover:shadow-lg transition-all duration-300"
        >
          <div class="flex gap-4">
            <!-- Product Image -->
            <div class="w-20 h-20 flex-shrink-0 rounded-lg bg-gradient-to-br from-indigo-100 to-purple-100 flex items-center justify-center">
              <span class="text-2xl">📦</span>
            </div>
            
            <!-- Product Details -->
            <div class="flex-1 min-w-0">
              <h3 class="font-bold text-slate-800 mb-1 truncate">
                {{ item.name }}
              </h3>
              <p class="text-lg font-bold text-indigo-600">
                ${{ item.price.toLocaleString() }}
              </p>
              <p class="text-xs text-slate-500 mt-1">
                Subtotal: ${{ (item.price * item.quantity).toLocaleString() }}
              </p>
            </div>
          </div>

          <!-- Quantity Controls -->
          <div class="flex items-center justify-between mt-4">
            <div class="flex items-center bg-slate-50 rounded-lg border border-slate-200 overflow-hidden">
              <button
                @click="cartStore.updateQuantity(item.id, item.quantity - 1)"
                class="px-3 py-2 text-slate-600 hover:bg-indigo-50 hover:text-indigo-600 transition-colors"
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
                class="px-3 py-2 text-slate-600 hover:bg-indigo-50 hover:text-indigo-600 transition-colors"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                </svg>
              </button>
            </div>
            
            <button
              @click="cartStore.removeFromCart(item.id)"
              class="p-2 text-red-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-all duration-200"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Footer with Summary -->
      <div v-if="cartStore.items.length > 0" class="border-t border-slate-200 p-6 bg-gradient-to-b from-white to-slate-50 space-y-4">
        <div class="space-y-2">
          <div class="flex justify-between text-slate-600">
            <span>Productos ({{ cartStore.totalItems }})</span>
            <span class="font-medium">${{ cartStore.subtotal.toLocaleString() }}</span>
          </div>
          
          <div class="flex justify-between text-slate-600">
            <span>IVA (19%)</span>
            <span class="font-medium">${{ cartStore.tax.toLocaleString() }}</span>
          </div>
          
          <div class="border-t border-slate-200 pt-2 flex justify-between text-lg font-bold text-slate-800">
            <span>Total</span>
            <span class="text-2xl text-indigo-600">${{ cartStore.total.toLocaleString() }}</span>
          </div>
        </div>

        <div v-if="error" class="rounded-lg bg-red-50 border border-red-200 p-3">
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
          Recibirás un correo con los detalles de tu compra
        </p>
      </div>
    </div>
  </transition>
</template>

<script setup>
import { ref } from 'vue';
import { useCartStore } from '../stores/cart';
import { useAuthStore } from '../stores/auth';

const props = defineProps({
  isOpen: {
    type: Boolean,
    required: true
  }
});

const emit = defineEmits(['close', 'checkout-success']);

const cartStore = useCartStore();
const authStore = useAuthStore();
const loading = ref(false);
const error = ref('');

const close = () => {
  emit('close');
};

const handleCheckout = async () => {
  if (!authStore.user) {
    error.value = 'Usuario no identificado. Por favor inicie sesión nuevamente.';
    return;
  }

  const userEmail = authStore.user.email || authStore.user.Email;
  
  if (!userEmail) {
    error.value = 'No se pudo obtener el email del usuario.';
    return;
  }

  loading.value = true;
  error.value = '';
  
  try {
    const saleData = await cartStore.checkout(userEmail);
    emit('checkout-success', saleData);
    close();
  } catch (err) {
    console.error('Checkout error:', err);
    
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

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.slide-enter-active,
.slide-leave-active {
  transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.slide-enter-from,
.slide-leave-to {
  transform: translateX(100%);
}

.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: rgba(148, 163, 184, 0.1);
  border-radius: 10px;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: rgba(99, 102, 241, 0.3);
  border-radius: 10px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: rgba(99, 102, 241, 0.5);
}
</style>
