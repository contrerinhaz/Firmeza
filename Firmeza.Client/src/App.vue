<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100">
    <nav v-if="authStore.isAuthenticated" class="fixed w-full z-50 glass transition-all duration-300">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-20 items-center">
          <div class="flex items-center gap-8">
            <div class="flex-shrink-0 flex items-center gap-2">
              <div class="w-10 h-10 bg-indigo-600 rounded-xl flex items-center justify-center text-white font-bold text-xl shadow-lg shadow-indigo-500/30">F</div>
              <h1 class="text-2xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-indigo-600 to-violet-600">Firmeza</h1>
            </div>
            <div class="hidden sm:flex sm:space-x-1">
              <router-link to="/catalog" class="px-4 py-2 rounded-lg text-sm font-medium text-slate-600 hover:text-indigo-600 hover:bg-indigo-50 transition-all duration-200" active-class="bg-indigo-50 text-indigo-700">
                Catálogo
              </router-link>
            </div>
          </div>
          <div class="flex items-center gap-3">
            <!-- Cart Button with Badge -->
            <button 
              @click="isCartOpen = true" 
              class="relative p-2.5 text-slate-600 hover:text-indigo-600 hover:bg-indigo-50 rounded-full transition-all duration-200"
              title="Ver Carrito"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
              </svg>
              <span 
                v-if="cartStore.totalItems > 0" 
                class="absolute -top-1 -right-1 w-5 h-5 bg-gradient-to-r from-indigo-600 to-violet-600 text-white text-xs font-bold rounded-full flex items-center justify-center ring-2 ring-white"
              >
                {{ cartStore.totalItems }}
              </span>
            </button>

            <UserProfile />
            <button 
              @click="logout" 
              class="p-2.5 text-slate-400 hover:text-red-500 transition-all duration-200 rounded-full hover:bg-red-50 hover:shadow-lg hover:shadow-red-100"
              title="Cerrar Sesión"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </nav>
    <main class="pt-28 pb-12 px-4 sm:px-6 lg:px-8 max-w-7xl mx-auto">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>

    <!-- Cart Drawer -->
    <CartDrawer 
      :isOpen="isCartOpen" 
      @close="isCartOpen = false" 
      @checkout-success="handleCheckoutSuccess"
    />

    <!-- Success Modal -->
    <SuccessModal 
      :show="showSuccessModal" 
      :orderId="lastOrderId"
      @close="showSuccessModal = false"
    />
  </div>
</template>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from './stores/auth';
import { useCartStore } from './stores/cart';
import UserProfile from './components/UserProfile.vue';
import CartDrawer from './components/CartDrawer.vue';
import SuccessModal from './components/SuccessModal.vue';

const authStore = useAuthStore();
const cartStore = useCartStore();
const isCartOpen = ref(false);
const showSuccessModal = ref(false);
const lastOrderId = ref(0);

const logout = () => {
  authStore.logout();
};

const handleCheckoutSuccess = (saleData) => {
  // Get order ID from sale data if available
  lastOrderId.value = saleData?.id || Math.floor(Math.random() * 10000);
  showSuccessModal.value = true;
};
</script>

