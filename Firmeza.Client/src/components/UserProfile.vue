<template>
  <div class="relative" ref="profileContainer">
    <!-- Profile Button -->
    <button 
      @click="toggleProfile" 
      class="flex items-center gap-3 px-4 py-2 bg-white/50 rounded-full border border-slate-200/50 hover:bg-white/80 hover:border-indigo-200 transition-all duration-300 hover:shadow-lg hover:shadow-indigo-100"
    >
      <div class="w-8 h-8 rounded-full bg-gradient-to-tr from-indigo-500 to-violet-500 flex items-center justify-center text-white text-xs font-bold ring-2 ring-white">
        {{ authStore.user?.username?.charAt(0).toUpperCase() || 'U' }}
      </div>
      <span class="text-sm font-medium text-slate-700">{{ authStore.user?.username || 'Cliente' }}</span>
      <svg 
        xmlns="http://www.w3.org/2000/svg" 
        class="h-4 w-4 text-slate-400 transition-transform duration-300"
        :class="{ 'rotate-180': isOpen }"
        fill="none" 
        viewBox="0 0 24 24" 
        stroke="currentColor"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
      </svg>
    </button>

    <!-- Dropdown Profile Card -->
    <transition name="dropdown">
      <div 
        v-if="isOpen" 
        class="absolute right-0 mt-3 w-[380px] max-w-[calc(100vw-2rem)] glass rounded-2xl shadow-2xl border border-white/20 overflow-hidden z-[60]"
      >
        <!-- Header -->
        <div class="bg-gradient-to-br from-indigo-600 to-violet-600 p-6 text-white">
          <div class="flex items-start gap-4">
            <div class="w-16 h-16 flex-shrink-0 rounded-full bg-white/20 backdrop-blur-sm flex items-center justify-center text-white text-2xl font-bold ring-4 ring-white/30">
              {{ authStore.user?.username?.charAt(0).toUpperCase() || 'U' }}
            </div>
            <div class="flex-1 min-w-0">
              <h3 class="text-xl font-bold truncate">{{ authStore.user?.username || 'Cliente' }}</h3>
              <p class="text-indigo-100 text-sm break-all">{{ authStore.user?.email || 'email@example.com' }}</p>
            </div>
          </div>
        </div>

        <!-- Stats -->
        <div class="grid grid-cols-2 gap-4 p-6 bg-white/40">
          <div class="text-center p-4 bg-white/60 rounded-xl border border-slate-200/50">
            <div class="text-2xl font-bold text-indigo-600">{{ userPurchases.length }}</div>
            <div class="text-xs text-slate-600 font-medium mt-1">Compras</div>
          </div>
          <div class="text-center p-4 bg-white/60 rounded-xl border border-slate-200/50">
            <div class="text-lg font-bold text-violet-600 break-words leading-tight">{{ formatCurrency(totalSpent) }}</div>
            <div class="text-xs text-slate-600 font-medium mt-1">Total Gastado</div>
          </div>
        </div>

        <!-- Recent Purchases -->
        <div class="p-6 bg-white/30">
          <h4 class="text-sm font-bold text-slate-700 mb-3 flex items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-indigo-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            Compras Recientes
          </h4>
          
          <div v-if="loading" class="text-center py-8">
            <div class="inline-block animate-spin rounded-full h-8 w-8 border-4 border-indigo-200 border-t-indigo-600"></div>
          </div>

          <div v-else-if="userPurchases.length === 0" class="text-center py-8">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-12 w-12 text-slate-300 mx-auto mb-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            <p class="text-sm text-slate-500">No hay compras aún</p>
          </div>

          <div v-else class="space-y-3 max-h-64 overflow-y-auto custom-scrollbar">
            <div 
              v-for="purchase in recentPurchases" 
              :key="purchase.id"
              @click="openSaleDetails(purchase)"
              class="p-3 bg-white/60 rounded-lg border border-slate-200/50 hover:border-indigo-300 hover:shadow-md transition-all duration-200 cursor-pointer group"
            >
              <div class="flex justify-between items-start gap-3">
                <div class="flex-1 min-w-0">
                  <div class="text-xs text-slate-500 mb-0.5">{{ formatDate(purchase.date) }}</div>
                  <div class="font-bold text-indigo-600 truncate group-hover:text-violet-600 transition-colors" :title="formatCurrency(purchase.total)">
                    {{ formatCurrency(purchase.total) }}
                  </div>
                  <div class="text-xs text-slate-600 mt-0.5">{{ purchase.saleDetails?.length || 0 }} productos</div>
                </div>
                <div class="flex-shrink-0 px-2 py-1 bg-green-100 text-green-700 text-[10px] font-bold uppercase tracking-wider rounded-full">
                  Exitosa
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>
    <!-- Sale Details Modal -->
    <SaleDetailsModal 
      :show="showDetailsModal" 
      :sale="selectedSale" 
      @close="showDetailsModal = false" 
    />
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { useAuthStore } from '../stores/auth';
import api from '../services/api';
import SaleDetailsModal from './SaleDetailsModal.vue';

const authStore = useAuthStore();
const isOpen = ref(false);
const profileContainer = ref(null);
const userPurchases = ref([]);
const showDetailsModal = ref(false);
const selectedSale = ref(null);

const toggleProfile = () => {
  isOpen.value = !isOpen.value;
  if (isOpen.value) {
    fetchUserPurchases();
  }
};

const closeProfile = (e) => {
  if (profileContainer.value && !profileContainer.value.contains(e.target)) {
    isOpen.value = false;
  }
};

const fetchUserPurchases = async () => {
  if (!authStore.user?.email) return;
  
  try {
    // Use the api service to ensure the token is sent
    const response = await api.get(`/Sales/user/${authStore.user.email}`);
    // Sort by date descending (newest first)
    userPurchases.value = response.data.sort((a, b) => new Date(b.date) - new Date(a.date));
  } catch (error) {
    console.error('Error fetching purchases:', error);
  }
};

const openSaleDetails = (sale) => {
  selectedSale.value = sale;
  showDetailsModal.value = true;
  isOpen.value = false; // Close dropdown when opening modal
};

const totalSpent = computed(() => {
  return userPurchases.value.reduce((sum, purchase) => sum + purchase.total, 0);
});

const recentPurchases = computed(() => {
  return userPurchases.value.slice(0, 5);
});

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value);
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('es-CO', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  }).format(date);
};

onMounted(() => {
  document.addEventListener('click', closeProfile);
  if (authStore.isAuthenticated) {
    fetchUserPurchases();
  }
});

onUnmounted(() => {
  document.removeEventListener('click', closeProfile);
});
</script>

<style scoped>
.glass {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
}

.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
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
