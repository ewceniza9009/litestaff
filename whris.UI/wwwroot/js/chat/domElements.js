export const chatInput = document.getElementById('chatInput');
export const sendButton = document.getElementById('sendButton');
export const chatHistoryEl = document.getElementById('chatHistory');
export const newChatButton = document.getElementById('newChatButton');
export const exportChatPdfButton = document.getElementById('exportChatPdfButton');
export const conversationListEl = document.getElementById('conversation-list');
export const conversationSearchInput = document.getElementById('conversationSearchInput');
export const pdfChartRenderContainer = document.getElementById('pdf-chart-render-container');
export const pdfMermaidRenderContainer = document.getElementById('pdf-mermaid-render-container');
export const singleChatSearchInput = document.getElementById('singleChatSearchInput');
export const toastContainer = document.getElementById('toast-container');

export const aiInsightsToggle = document.getElementById('aiInsightsToggle');
export const aiInsightsToggleLabel = document.getElementById('aiInsightsToggleTextLabel');

export let tooltip = null;
export function initializeTooltip(d3Instance) {
    if (d3Instance && d3Instance.select) {
        tooltip = d3Instance.select("body").append("div")
            .attr("id", "tooltip")
            .attr("class", "tooltip")                             
            .style("opacity", 0)
            .style("position", "absolute")
            .style("background-color", "white")
            .style("border", "solid")
            .style("border-width", "1px")
            .style("border-radius", "5px")
            .style("padding", "10px")
            .style("pointer-events", "none");                 
    } else if (document.getElementById('tooltip')) {
        tooltip = d3Instance.select("#tooltip");
    } else {
        console.warn('Tooltip element #tooltip not found and D3 instance not provided for creation.');
    }
}
