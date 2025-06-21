import { toastContainer } from './domElements.js';

export function showToast(message, type = 'info') {
    if (!toastContainer) {
        console.error('Toast container not found.');
        return;
    }
    const toast = document.createElement('div');
    toast.className = `toast-message ${type}`;
    toast.textContent = message;
    toastContainer.appendChild(toast);
    setTimeout(() => toast.classList.add('show'), 10);                     
    setTimeout(() => {
        toast.classList.remove('show');
        toast.addEventListener('transitionend', () => toast.remove());
    }, 5000);
}
