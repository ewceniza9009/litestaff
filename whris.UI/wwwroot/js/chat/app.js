import { initializeConfig } from './config.js';
import {
    chatInput, sendButton, newChatButton, exportChatPdfButton,
    conversationSearchInput, singleChatSearchInput, initializeTooltip,
    aiInsightsToggle, aiInsightsToggleLabel         
} from './domElements.js';
import {
    sendMessageHandler, startNewChatHandler, loadConversationsHandler,
    exportChatToPDFHandler, handleConversationSearch, handleSingleChatSearch,
    handleChatInputKeypress, autoResizeChatInputHandler
} from './eventHandlers.js';
import { setupSqlEditModal } from './sqlModalHandler.js';
import { state, setAiInsightsEnabled } from './state.js';                 
import { showToast } from './toastService.js';         

document.addEventListener('DOMContentLoaded', () => {
    initializeConfig();             

    if (window.d3) {
        initializeTooltip(window.d3);
    } else {
        console.error("D3.js is not loaded. Tooltips will not work.");
    }

    if (window.mermaid) {             
        mermaid.initialize({
            startOnLoad: false,                                 
            theme: 'neutral',                   
        });
        if (typeof window.mermaid.run !== 'function') {
            console.error("Mermaid.js loaded, but mermaid.run() is not available. Diagrams might not render dynamically.");
        }
    } else {
        console.warn("Mermaid.js not loaded. Diagrams will not render.");
    }

    if (sendButton) sendButton.addEventListener('click', sendMessageHandler);
    if (chatInput) {
        chatInput.addEventListener('keypress', handleChatInputKeypress);
        chatInput.addEventListener('input', autoResizeChatInputHandler);             
    }
    if (newChatButton) newChatButton.addEventListener('click', startNewChatHandler);
    if (exportChatPdfButton) exportChatPdfButton.addEventListener('click', exportChatToPDFHandler);
    if (conversationSearchInput) conversationSearchInput.addEventListener('input', handleConversationSearch);
    if (singleChatSearchInput) singleChatSearchInput.addEventListener('input', handleSingleChatSearch);

    if (aiInsightsToggle && aiInsightsToggleLabel) {
        aiInsightsToggle.checked = state.aiInsightsEnabled;                             
        aiInsightsToggleLabel.textContent = state.aiInsightsEnabled ? 'AI Insights: ON' : 'AI Insights: OFF';

        aiInsightsToggle.addEventListener('change', (event) => {
            const isEnabled = event.target.checked;
            setAiInsightsEnabled(isEnabled);                 
            aiInsightsToggleLabel.textContent = isEnabled ? 'AI Insights: ON' : 'AI Insights: OFF';
            showToast(`AI Insights ${isEnabled ? 'Enabled' : 'Disabled'}`, 'info');
        });
    } else {
        console.warn("AI Insights toggle or label element not found.");
    }

    setupSqlEditModal();                                 
    loadConversationsHandler();             
    startNewChatHandler();                                 
    autoResizeChatInputHandler();                         

    console.log("Chat application initialized...");

    if (!window.jspdf) console.warn("jsPDF is not loaded. PDF export features might fail.");
    if (!window.canvg) console.warn("canvg is not loaded. Chart to PDF/image features might fail.");
    if (!window.CodeMirror) console.warn("CodeMirror is not loaded. SQL editor will be a plain textarea.");
    if (!window.Prism) console.warn("Prism.js is not loaded. SQL syntax highlighting will not work.");
    if (!window.html2canvas) console.warn("html2canvas is not loaded. Share message as image will not work.");
    if (!window.L) console.warn("Leaflet (L) is not loaded. Map rendering will fail.");
    if (!window.mermaid) console.warn("Mermaid.js not loaded. Diagrams will not render.");                     
});
