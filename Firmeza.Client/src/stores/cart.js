import { defineStore } from 'pinia';
import api from '../services/api';

export const useCartStore = defineStore('cart', {
    state: () => ({
        items: JSON.parse(localStorage.getItem('cartItems')) || []
    }),
    getters: {
        totalItems: (state) => state.items.reduce((total, item) => total + item.quantity, 0),
        subtotal: (state) => state.items.reduce((total, item) => total + (item.price * item.quantity), 0),
        tax: (state) => state.subtotal * 0.19, // 19% VAT
        total: (state) => state.subtotal + state.tax
    },
    actions: {
        addToCart(product) {
            const existingItem = this.items.find(item => item.id === product.id);
            if (existingItem) {
                existingItem.quantity++;
            } else {
                this.items.push({ ...product, quantity: 1 });
            }
            this.saveCart();
        },
        removeFromCart(productId) {
            this.items = this.items.filter(item => item.id !== productId);
            this.saveCart();
        },
        updateQuantity(productId, quantity) {
            const item = this.items.find(item => item.id === productId);
            if (item) {
                item.quantity = quantity;
                if (item.quantity <= 0) {
                    this.removeFromCart(productId);
                } else {
                    this.saveCart();
                }
            }
        },
        clearCart() {
            this.items = [];
            this.saveCart();
        },
        saveCart() {
            localStorage.setItem('cartItems', JSON.stringify(this.items));
        },
        async checkout(userEmail) {
            try {
                const saleData = {
                    UserName: userEmail, // Backend usa PascalCase y UserName es el email
                    Details: this.items.map(item => ({
                        ProductId: item.id, // PascalCase requerido por el backend
                        Quantity: item.quantity // PascalCase requerido por el backend
                    }))
                };

                console.log('Sending sale data:', saleData);
                const response = await api.post('/Sales', saleData);
                this.clearCart();
                return response.data;
            } catch (error) {
                console.error('Checkout failed', error);
                console.error('Error details:', error.response?.data);
                throw error;
            }
        }
    }
});
