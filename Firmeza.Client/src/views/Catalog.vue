<template>
  <div>
    <!-- Hero Section -->
    <div class="glass-card p-8 mb-10 text-center">
      <h1 class="text-4xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-indigo-600 to-violet-600 mb-3">
        Catálogo de Productos
      </h1>
      <p class="text-slate-600 text-lg">
        Descubre nuestros productos de la más alta calidad
      </p>
    </div>
    
    <!-- Loading State -->
    <div v-if="loading" class="flex justify-center items-center py-20">
      <div class="glass-card p-8 text-center">
        <svg class="animate-spin h-12 w-12 text-indigo-600 mx-auto mb-4" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <p class="text-lg text-slate-600">Cargando productos...</p>
      </div>
    </div>
    
    <!-- Error State -->
    <div v-else-if="error" class="glass-card p-8 text-center border-red-200 bg-red-50/80">
      <p class="text-lg text-red-600">{{ error }}</p>
    </div>
    
    <!-- Products Grid -->
    <div v-else>
      <!-- Empty State -->
      <div v-if="allProducts.length === 0" class="glass-card p-12 text-center">
        <div class="text-6xl mb-4">📦</div>
        <h3 class="text-xl font-semibold text-slate-700 mb-2">No hay productos disponibles</h3>
        <p class="text-slate-500">Vuelve pronto para ver nuevos productos</p>
      </div>

      <!-- Products -->
      <div v-else>
        <!-- Products Info -->
        <div class="flex justify-between items-center mb-6">
          <p class="text-sm text-slate-600">
            Mostrando {{ startIndex + 1 }}-{{ endIndex }} de {{ allProducts.length }} productos
          </p>
          <p class="text-sm text-slate-600">
            Página {{ currentPage }} de {{ totalPages }}
          </p>
        </div>

        <!-- Products Grid -->
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-8">
          <div
            v-for="product in paginatedProducts"
            :key="product.id"
            class="group glass-card overflow-hidden hover:shadow-2xl hover:scale-[1.02] transition-all duration-300"
          >
            <!-- Product Image Placeholder -->
            <div class="h-56 bg-gradient-to-br from-indigo-100 to-purple-100 flex items-center justify-center relative overflow-hidden">
              <div class="absolute inset-0 bg-gradient-to-br from-indigo-500/10 to-violet-500/10 group-hover:scale-110 transition-transform duration-500"></div>
              <span class="text-6xl z-10 group-hover:scale-110 transition-transform duration-300">📦</span>
            </div>
            
            <!-- Product Info -->
            <div class="p-5 space-y-3">
              <h3 class="text-lg font-bold text-slate-800 line-clamp-1 group-hover:text-indigo-600 transition-colors">
                {{ product.name }}
              </h3>
              
              <p class="text-sm text-slate-500 line-clamp-2 min-h-[2.5rem]">
                {{ product.description || 'Producto de alta calidad' }}
              </p>
              
              <div class="flex items-center justify-between pt-2">
                <div>
                  <p class="text-2xl font-bold text-indigo-600">
                    ${{ product.price.toLocaleString() }}
                  </p>
                  <p class="text-xs text-slate-400 flex items-center gap-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                    Stock: {{ product.quantity }}
                  </p>
                </div>
                
                <button
                  @click="addToCart(product)"
                  :disabled="product.quantity <= 0"
                  class="p-3 rounded-xl bg-indigo-600 text-white hover:bg-indigo-700 disabled:bg-slate-300 disabled:cursor-not-allowed transition-all duration-300 hover:scale-110 active:scale-95 shadow-lg shadow-indigo-500/30 hover:shadow-indigo-500/50"
                  :class="{ 'animate-pulse': product.quantity > 0 }"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Pagination Controls -->
        <div class="flex justify-center items-center gap-4 mt-8">
          <button
            @click="goToPage(currentPage - 1)"
            :disabled="currentPage === 1"
            class="px-6 py-3 bg-white/50 border border-slate-200 rounded-xl font-medium text-slate-700 hover:bg-indigo-50 hover:border-indigo-300 hover:text-indigo-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-300"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
            </svg>
          </button>

          <div class="flex gap-2">
            <button
              v-for="page in visiblePages"
              :key="page"
              @click="goToPage(page)"
              :class="[
                'px-4 py-2 rounded-xl font-medium transition-all duration-300',
                page === currentPage
                  ? 'bg-indigo-600 text-white shadow-lg shadow-indigo-500/30'
                  : 'bg-white/50 border border-slate-200 text-slate-700 hover:bg-indigo-50 hover:border-indigo-300 hover:text-indigo-600'
              ]"
            >
              {{ page }}
            </button>
          </div>

          <button
            @click="goToPage(currentPage + 1)"
            :disabled="currentPage === totalPages"
            class="px-6 py-3 bg-white/50 border border-slate-200 rounded-xl font-medium text-slate-700 hover:bg-indigo-50 hover:border-indigo-300 hover:text-indigo-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-300"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '../services/api';
import { useCartStore } from '../stores/cart';

const allProducts = ref([]);
const loading = ref(true);
const error = ref('');
const cartStore = useCartStore();

// Pagination
const currentPage = ref(1);
const itemsPerPage = 12; // Mostrar 12 productos por página (3 filas de 4)

const totalPages = computed(() => Math.ceil(allProducts.value.length / itemsPerPage));
const startIndex = computed(() => (currentPage.value - 1) * itemsPerPage);
const endIndex = computed(() => Math.min(startIndex.value + itemsPerPage, allProducts.value.length));

const paginatedProducts = computed(() => {
  return allProducts.value.slice(startIndex.value, endIndex.value);
});

const visiblePages = computed(() => {
  const pages = [];
  const maxVisible = 5;
  let start = Math.max(1, currentPage.value - Math.floor(maxVisible / 2));
  let end = Math.min(totalPages.value, start + maxVisible - 1);
  
  if (end - start < maxVisible - 1) {
    start = Math.max(1, end - maxVisible + 1);
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  
  return pages;
});

const goToPage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

const fetchProducts = async () => {
  try {
    const response = await api.get('/Products');
    allProducts.value = response.data;
  } catch (err) {
    error.value = 'Error al cargar productos.';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const addToCart = (product) => {
  cartStore.addToCart(product);
  // Visual feedback could be enhanced with a toast notification
  console.log('Producto agregado:', product.name);
};

onMounted(() => {
  fetchProducts();
});
</script>
