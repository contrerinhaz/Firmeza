<template>
  <transition name="modal-fade">
    <div v-if="show" class="fixed inset-0 z-[100] flex items-center justify-center p-4 sm:p-6">
      <!-- Backdrop with lighter glass effect -->
      <div class="absolute inset-0 bg-slate-900/30 backdrop-blur-md transition-opacity" @click="close"></div>
      
      <!-- Modal Content -->
      <div class="relative bg-white rounded-3xl shadow-2xl max-w-2xl w-full overflow-hidden flex flex-col max-h-[85vh] transform transition-all scale-100 my-auto">
        
        <!-- Header with Glass Effect -->
        <div class="relative bg-gradient-to-r from-indigo-600 via-violet-600 to-purple-600 p-8 text-white shrink-0 overflow-hidden">
          <!-- Decorative circles -->
          <div class="absolute top-0 right-0 -mt-10 -mr-10 w-40 h-40 bg-white/10 rounded-full blur-2xl"></div>
          <div class="absolute bottom-0 left-0 -mb-10 -ml-10 w-40 h-40 bg-black/10 rounded-full blur-2xl"></div>
          
          <div class="relative flex justify-between items-start z-10">
            <div>
              <div class="flex items-center gap-3 mb-2">
                <div class="p-2 bg-white/20 rounded-lg backdrop-blur-md">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                  </svg>
                </div>
                <h2 class="text-2xl font-bold tracking-tight">Detalles de Compra</h2>
              </div>
              <p class="text-indigo-100 font-medium opacity-90">Orden <span class="font-mono bg-white/20 px-2 py-0.5 rounded text-sm">#{{ sale?.id }}</span></p>
            </div>
            <button 
              @click="close"
              class="p-2 bg-white/10 hover:bg-white/20 rounded-full transition-all duration-200 text-white hover:rotate-90"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Scrollable Content -->
        <div class="p-8 overflow-y-auto custom-scrollbar bg-slate-50/50">
          <!-- Info Grid -->
          <div class="grid grid-cols-2 gap-6 mb-8">
            <div class="p-5 bg-white rounded-2xl border border-slate-100 shadow-sm hover:shadow-md transition-shadow">
              <div class="flex items-center gap-3 mb-2">
                <div class="p-2 bg-blue-50 text-blue-600 rounded-lg">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                </div>
                <p class="text-xs text-slate-500 uppercase font-bold tracking-wider">Fecha de Compra</p>
              </div>
              <p class="text-slate-800 font-bold text-lg pl-1">{{ formatDate(sale?.date) }}</p>
            </div>
            
            <div class="p-5 bg-white rounded-2xl border border-slate-100 shadow-sm hover:shadow-md transition-shadow">
              <div class="flex items-center gap-3 mb-2">
                <div class="p-2 bg-emerald-50 text-emerald-600 rounded-lg">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <p class="text-xs text-slate-500 uppercase font-bold tracking-wider">Estado</p>
              </div>
              <div class="pl-1">
                <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-bold bg-emerald-100 text-emerald-700 border border-emerald-200">
                  Completada
                </span>
              </div>
            </div>
          </div>

          <!-- Products List -->
          <div class="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden mb-8">
            <div class="px-6 py-4 border-b border-slate-100 bg-slate-50/50 flex justify-between items-center">
              <h3 class="text-lg font-bold text-slate-800">Productos Adquiridos</h3>
              <span class="text-xs font-medium text-slate-500 bg-slate-200 px-2 py-1 rounded-md">{{ sale?.saleDetails?.length || 0 }} items</span>
            </div>
            
            <table class="w-full text-sm text-left">
              <thead class="bg-slate-50 text-slate-500 font-semibold border-b border-slate-100">
                <tr>
                  <th class="px-6 py-4">Producto</th>
                  <th class="px-6 py-4 text-center">Cant.</th>
                  <th class="px-6 py-4 text-right">Precio Unit.</th>
                  <th class="px-6 py-4 text-right">Total</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                <tr v-for="detail in sale?.saleDetails" :key="detail.id" class="hover:bg-slate-50 transition-colors group">
                  <td class="px-6 py-4">
                    <div class="flex items-center gap-3">
                      <div class="w-10 h-10 rounded-lg bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold text-lg group-hover:bg-indigo-100 transition-colors">
                        📦
                      </div>
                      <span class="font-medium text-slate-700 group-hover:text-indigo-700 transition-colors">
                        {{ detail.productName || 'Producto #' + detail.productId }}
                      </span>
                    </div>
                  </td>
                  <td class="px-6 py-4 text-center">
                    <span class="bg-slate-100 text-slate-600 px-2.5 py-1 rounded-md font-bold text-xs">
                      x{{ detail.quantity }}
                    </span>
                  </td>
                  <td class="px-6 py-4 text-right text-slate-500 font-medium">{{ formatCurrency(detail.unitPrice) }}</td>
                  <td class="px-6 py-4 text-right font-bold text-slate-800">{{ formatCurrency(detail.subtotal) }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Totals -->
          <div class="flex justify-end">
            <div class="w-full sm:w-80 bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
              <div class="space-y-3">
                <div class="flex justify-between text-slate-500 text-sm">
                  <span>Subtotal</span>
                  <span class="font-medium">{{ formatCurrency((sale?.total || 0) - (sale?.vat || 0)) }}</span>
                </div>
                <div class="flex justify-between text-slate-500 text-sm">
                  <span>IVA (19%)</span>
                  <span class="font-medium">{{ formatCurrency(sale?.vat || 0) }}</span>
                </div>
                <div class="my-3 border-t border-dashed border-slate-200"></div>
                <div class="flex justify-between items-end">
                  <span class="text-slate-800 font-bold">Total Pagado</span>
                  <span class="text-2xl font-black text-transparent bg-clip-text bg-gradient-to-r from-indigo-600 to-violet-600">
                    {{ formatCurrency(sale?.total || 0) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-6 border-t border-slate-100 bg-white shrink-0 flex justify-between items-center">
          <p class="text-xs text-slate-400">
            Comprobante digital generado el {{ formatDate(sale?.date) }}
          </p>
          <button 
            @click="close"
            class="px-8 py-2.5 bg-slate-900 text-white font-medium rounded-xl hover:bg-slate-800 hover:shadow-lg hover:-translate-y-0.5 transition-all duration-200 active:scale-95"
          >
            Cerrar
          </button>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup>
const props = defineProps({
  show: Boolean,
  sale: Object
});

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value);
};

const formatDate = (dateString) => {
  if (!dateString) return '';
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
</script>

<style scoped>
.modal-fade-enter-active,
.modal-fade-leave-active {
  transition: opacity 0.2s ease;
}

.modal-fade-enter-from,
.modal-fade-leave-to {
  opacity: 0;
}

.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: #f1f5f9;
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 10px;
}

.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}
</style>
