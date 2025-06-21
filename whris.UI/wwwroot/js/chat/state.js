const getInitialInsightsState = () => {
    try {
        const storedState = localStorage.getItem('aiInsightsEnabled');
        if (storedState === null) {                                         
            localStorage.setItem('aiInsightsEnabled', 'true');
            return true;
        }
        return storedState === 'true';                     
    } catch (e) {
        console.error("Error accessing localStorage for AI Insights state:", e);
        return true;
    }
};

export const state = {
    conversationHistory: [],
    lastSuccessfulQuery: null,
    currentConversationId: null,
    isSending: false,
    isSavingNewConversation: false,
    aiInsightsEnabled: getInitialInsightsState(),
};

export function updateConversationHistory(newHistory) {
    state.conversationHistory = newHistory;
}

export function addMessageToHistory(message) {
    state.conversationHistory.push(message);
}

export function setLastSuccessfulQuery(query) {
    state.lastSuccessfulQuery = query;
}

export function setCurrentConversationId(id) {
    state.currentConversationId = id;
}

export function setIsSending(sending) {
    state.isSending = sending;
}

export function setIsSavingNewConversation(saving) {
    state.isSavingNewConversation = saving;
}

export function setAiInsightsEnabled(enabled) {
    state.aiInsightsEnabled = enabled;
    try {
        localStorage.setItem('aiInsightsEnabled', enabled ? 'true' : 'false');
    } catch (e) {
        console.error("Error saving AI Insights state to localStorage:", e);
    }
}

export function findMessageByTimestamp(timestamp) {
    return state.conversationHistory.find(msg => msg.timestamp === timestamp && msg.role === 'model');
}

export function updateMessageFeedback(timestamp, feedback) {
    const message = findMessageByTimestamp(timestamp);
    if (message) {
        message.feedback = feedback;
    }
}

export function getLatestUserMessageContent() {
    const userMessages = state.conversationHistory.filter(m => m.role === 'user' && m.content);
    return userMessages.pop()?.content || "the user's last question";
}

export function removeMessageByTimestamp(timestamp) {
    const initialLength = state.conversationHistory.length;
    state.conversationHistory = state.conversationHistory.filter(msg => msg.timestamp !== timestamp);
    return state.conversationHistory.length < initialLength;
}
