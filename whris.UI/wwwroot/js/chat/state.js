function safeGetItem(key, defaultValue) {
    try {
        if (typeof window !== 'undefined' && 'localStorage' in window) {
            const val = window.localStorage.getItem(key);
            return val !== null ? val : defaultValue;
        }
    } catch {
        // Storage access blocked or restricted in current context
    }
    return defaultValue;
}

function safeSetItem(key, value) {
    try {
        if (typeof window !== 'undefined' && 'localStorage' in window) {
            window.localStorage.setItem(key, value);
        }
    } catch {
        // Storage access blocked or restricted in current context
    }
}

const getInitialInsightsState = () => {
    const storedState = safeGetItem('aiInsightsEnabled', 'true');
    return storedState === 'true';
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
    safeSetItem('aiInsightsEnabled', enabled ? 'true' : 'false');
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
