import { requestVerificationToken } from './config.js';
import { state } from './state.js';                     

async function fetchWithAuth(url, options = {}) {
    const headers = {
        'Content-Type': 'application/json',
        'RequestVerificationToken': requestVerificationToken,
        ...options.headers,
    };
    return fetch(url, { ...options, headers });
}

export async function saveConversationAPI(conversationId, history) {
    return fetchWithAuth('?handler=SaveConversation', {
        method: 'POST',
        body: JSON.stringify({
            Id: conversationId,
            History: history
        })
    });
}

export async function sendMessageAPI(question, history, lastSqlQuery) {
    return fetchWithAuth('?handler=SendMessage', {
        method: 'POST',
        body: JSON.stringify({
            question: question,
            history: history.slice(-10),                         
            lastSqlQuery: lastSqlQuery,
            enableInsights: state.aiInsightsEnabled
        })
    });
}

export async function loadConversationsAPI() {
    return fetchWithAuth('?handler=Conversations');
}

export async function togglePinConversationAPI(id) {
    return fetchWithAuth(`?handler=TogglePin&id=${id}`, {
        method: 'POST'
    });
}

export async function updateConversationTitleAPI(id, newTitle) {
    return fetchWithAuth('?handler=UpdateTitle', {
        method: 'POST',
        body: JSON.stringify({ Id: id, NewTitle: newTitle })
    });
}

export async function getConversationAPI(id) {
    return fetchWithAuth(`?handler=Conversation&id=${id}`);
}

export async function deleteConversationAPI(id) {
    return fetchWithAuth(`?handler=DeleteConversation&id=${id}`, {
        method: 'POST'
    });
}

export async function summarizeDataAPI(tableData, userQuestion) {
    return fetchWithAuth('?handler=SummarizeData', {
        method: 'POST',
        body: JSON.stringify({
            tableData: tableData,
            userQuestion: userQuestion
        })
    });
}
