import { chatInput, sendButton, newChatButton, exportChatPdfButton, conversationSearchInput, singleChatSearchInput, conversationListEl, chatHistoryEl } from './domElements.js';
import { state, updateConversationHistory, addMessageToHistory, setLastSuccessfulQuery, setCurrentConversationId, setIsSending, setIsSavingNewConversation, getLatestUserMessageContent, removeMessageByTimestamp } from './state.js';
import { saveConversationAPI, sendMessageAPI, loadConversationsAPI, togglePinConversationAPI, updateConversationTitleAPI, getConversationAPI, deleteConversationAPI, summarizeDataAPI } from './apiService.js';
import { appendMessage, displayWelcomeMessage, updateConversationList, setSendingState, showThinkingIndicator, removeThinkingIndicator, clearChatUI, markConversationActive, updateConversationListItemTitle, showSummaryThinkingIndicator, removeSummaryThinkingIndicator } from './uiService.js';
import { showToast } from './toastService.js';
import { exportChatToPDF, escapeRegExp, formatTimestamp } from './utils.js';
import { openSqlEditor, setupSqlEditModal } from './sqlModalHandler.js';    

let isSavingNewConversationLock = false;                         

export async function saveCurrentConversationHandler() {
    const initialGlobalCurrentConversationId = state.currentConversationId;
    if (state.conversationHistory.length <= 1 && !initialGlobalCurrentConversationId) {
        return;
    }

    if (!initialGlobalCurrentConversationId) {                     
        if (isSavingNewConversationLock) {
            return;
        }
        isSavingNewConversationLock = true;             
    }
    try {
        const response = await saveConversationAPI(initialGlobalCurrentConversationId, state.conversationHistory);

        if (response.ok) {
            const savedConversation = await response.json();
            if (!initialGlobalCurrentConversationId) {                     
                if (savedConversation && savedConversation.id && savedConversation.id > 0) {
                    if (!state.currentConversationId || state.currentConversationId === initialGlobalCurrentConversationId) {
                        setCurrentConversationId(savedConversation.id);
                    } else {
                    }
                    await loadConversationsHandler();         
                } else {
                    console.error('[SAVE_CONV_HANDLER] CRITICAL ERROR: New conversation was supposedly saved, but server returned an invalid Id:', savedConversation ? savedConversation.id : 'savedConversation was null/undefined');
                }
            } else {                         
                if (savedConversation && savedConversation.title) {
                     updateConversationListItemTitle(initialGlobalCurrentConversationId, savedConversation.title);
                }
            }
        } else {
            console.error('Failed to save conversation. Status:', response.status, 'Response:', await response.text());
            showToast('Failed to save conversation.', 'error');
        }
    } catch (error) {
        console.error('Error saving conversation:', error);
        showToast('Error saving conversation.', 'error');
    } finally {
        if (!initialGlobalCurrentConversationId) {                                         
            isSavingNewConversationLock = false;
        }
    }
}

export async function sendMessageHandler() {
    if (!chatInput || !sendButton) return;
    let question = chatInput.value.trim();
    if (question === '' || state.isSending) return;

    setIsSending(true);
    setSendingState(true);

    const userMessage = {
        role: 'user',
        content: question,
        timestamp: new Date().toISOString()
    };
    appendMessage('user', userMessage);
    addMessageToHistory(userMessage);

    chatInput.value = '';
    chatInput.focus();
    autoResizeChatInputHandler();                 

    showThinkingIndicator();

    try {
        const response = await sendMessageAPI(question, state.conversationHistory, state.lastSuccessfulQuery);
        removeThinkingIndicator();

        if (!response.ok) {
            let errorText = `HTTP error! status: ${response.status}`;
            try {
                const errorData = await response.json();
                errorText = errorData.reply || errorData.message || errorText;
            } catch (e) {                              }
            throw new Error(errorText);
        }

        const data = await response.json();
        const botMessage = {
            role: 'model',
            content: data.reply,
            timestamp: new Date().toISOString(),
            insights: data.insights,
            suggestedVisualizations: data.suggestedVisualizations,
            clarificationQuestions: data.clarificationQuestions,
            executedQueries: data.executedQueries,
            chartData: data.chartData,
            chartType: data.chartType,
            tableData: data.tableData,
            sql: data.sql,
            mapQuery: data.mapQuery,
            mapLatitude: data.mapLatitude,
            mapLongitude: data.mapLongitude,
            mapZoom: data.mapZoom,
            suggestedQuestions: data.suggestedQuestions,
            feedback: null,
            isCrosstab: data.isCrosstab,
            isPivot: data.isPivot,
            mermaidDiagramSyntax: data.mermaidDiagramSyntax,
            isTree: data.isTree || false,                     
            treeData: data.isTree ? data.treeData : null,                         
            aiTreeConfig: data.isTree ? data.aiTreeConfig : null                         
        };

        appendMessage('bot', botMessage);
        addMessageToHistory(botMessage);

        if (data.sql) {
            setLastSuccessfulQuery(data.sql);
        }
        await saveCurrentConversationHandler();

    } catch (error) {
        console.error('Error in sendMessageHandler:', error);
        removeThinkingIndicator();
        const errorMessageContent = error.message.includes("Failed to fetch") ?
            'Sorry, I couldn\'t connect to the server. Please check your connection and try again.' :
            `Sorry, something went wrong: ${error.message}`;

        const errorMessage = {
            role: 'model',
            content: errorMessageContent,
            timestamp: new Date().toISOString()
        };
        appendMessage('bot', errorMessage);
        addMessageToHistory(errorMessage);
        showToast('An error occurred while sending the message.', 'error');
    } finally {
        setIsSending(false);
        setSendingState(false);
    }
}

export function startNewChatHandler() {
    clearChatUI();
    updateConversationHistory([]);
    setLastSuccessfulQuery(null);
    setCurrentConversationId(null);
    setIsSavingNewConversation(false);                 
    isSavingNewConversationLock = false;             
    displayWelcomeMessage();
    if (chatInput) {
        chatInput.focus();
        autoResizeChatInputHandler();         
    }
}

export async function loadConversationsHandler() {
    try {
        const response = await loadConversationsAPI();
        if (!response.ok) {
            throw new Error(`Failed to load conversations: ${response.status}`);
        }
        const conversations = await response.json();
        updateConversationList(conversations, state.currentConversationId);
    } catch (error) {
        console.error('Failed to load conversations:', error);
        showToast('Failed to load conversations.', 'error');
        if (conversationListEl) conversationListEl.innerHTML = `<li class="empty-state" style="padding: 20px; text-align: center; color: #cb3f3f;">Failed to load chats.</li>`;

    }
}

export async function togglePinConversationHandler(id) {
    try {
        const response = await togglePinConversationAPI(id);
        if (response.ok) {
            showToast('Pin status updated.', 'success');
            await loadConversationsHandler();                 
        } else {
            showToast('Failed to update pin status.', 'error');
        }
    } catch (error) {
        console.error('Error toggling pin:', error);
        showToast('An error occurred while pinning.', 'error');
    }
}

export async function enableTitleEditHandler(titleSpan, id, originalTitle) {
    titleSpan.classList.add('editing-title');                     
    titleSpan.contentEditable = true;
    titleSpan.focus();
    const selection = window.getSelection();
    const range = document.createRange();
    range.selectNodeContents(titleSpan);
    selection.removeAllRanges();
    selection.addRange(range);

    const saveOnEnter = async (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            await saveTitle();
        }
    };
    const saveOnBlur = async () => {
        await saveTitle();
    };

    async function saveTitle() {
        titleSpan.removeEventListener('blur', saveOnBlur);
        titleSpan.removeEventListener('keydown', saveOnEnter);
        titleSpan.contentEditable = false;
        titleSpan.classList.remove('editing-title');
        const newTitle = titleSpan.textContent.trim();

        if (newTitle && newTitle !== originalTitle) {
            try {
                const response = await updateConversationTitleAPI(id, newTitle);
                if (!response.ok) {
                    titleSpan.textContent = originalTitle;             
                    showToast('Failed to update title.', 'error');
                } else {
                    const updatedConv = await response.json();
                    titleSpan.textContent = updatedConv.NewTitle || newTitle;                         
                    showToast('Title updated successfully.', 'success');
                }
            } catch (error) {
                console.error('Error updating title:', error);
                titleSpan.textContent = originalTitle;             
                showToast('An error occurred while updating the title.', 'error');
            }
        } else {
            titleSpan.textContent = originalTitle;                         
        }
    }
    titleSpan.addEventListener('keydown', saveOnEnter);
    titleSpan.addEventListener('blur', saveOnBlur);
}

export async function selectConversationHandler(id) {
    if (state.isSending) {
        showToast("Please wait for the current message to send.", "warning");
        return;
    }
    try {
        const response = await getConversationAPI(id);
        if (!response.ok) {
            const errorText = await response.text();
            console.error("Failed to load conversation. Status:", response.status, "Response:", errorText);
            showToast('Failed to load conversation details.', 'error');
            return;
        }

        const data = await response.json();
        setCurrentConversationId(data.id);
        updateConversationHistory(data.history || []);
        setLastSuccessfulQuery(data.lastSqlQuery);
        clearChatUI();                         
        if (state.conversationHistory && typeof state.conversationHistory.forEach === 'function') {
            state.conversationHistory.forEach(message => {
                appendMessage(message.role, message);
            });
            if (state.conversationHistory.length === 0) {                     
                displayWelcomeMessage();                 
            }
        } else {
            console.error("[SELECT_CONV_HANDLER] conversationHistory is not an array or is undefined after fetch:", state.conversationHistory);
            displayWelcomeMessage();                         
        }
        markConversationActive(id);
        if (chatInput) autoResizeChatInputHandler();


    } catch (error) {
        console.error('Error selecting conversation:', error);
        showToast('Could not load the selected chat.', 'error');
    }
}

export async function deleteConversationHandler(id, title) {
    if (!window.confirm(`Are you sure you want to delete "${title}"?`)) return;
    try {
        const response = await deleteConversationAPI(id);
        if (!response.ok) {
             throw new Error(`Failed to delete: ${response.status}`);
        }
        showToast(`Conversation "${title}" deleted.`, 'success');
        if (id === state.currentConversationId) {
            startNewChatHandler();                                     
        }
        await loadConversationsHandler();         
    } catch (error) {
        console.error('Error deleting conversation:', error);
        showToast('Error deleting conversation.', 'error');
    }
}

export async function summarizeDataHandler(tableDataToSummarize) {
    if (!tableDataToSummarize || tableDataToSummarize.length === 0) {
        showToast("No data to summarize.", "info");
        return;
    }

    setIsSending(true);
    setSendingState(true);

    showSummaryThinkingIndicator();

    try {
        const lastUserQuestion = getLatestUserMessageContent();
        const response = await summarizeDataAPI(tableDataToSummarize, lastUserQuestion);

        setIsSending(false);
        setSendingState(false);

        removeSummaryThinkingIndicator();

        if (!response.ok) throw new Error('Failed to get summary from server.');
        const summaryData = await response.json();

        const summaryMessage = {
            role: 'model',
            content: summaryData.reply,
            timestamp: new Date().toISOString(),
        };
        appendMessage('bot', summaryMessage);
        addMessageToHistory(summaryMessage);
        await saveCurrentConversationHandler();

    } catch (error) {
        console.error('Error summarizing data:', error);
        removeSummaryThinkingIndicator();
        showToast('Could not summarize the data: ' + error.message, 'error');
    }
}

export function rerunMessageHandler(content) {
    if (chatInput && content) {
        chatInput.value = content;
        chatInput.focus();
        autoResizeChatInputHandler();             
        sendMessageHandler();             
    } else {
        showToast('Could not rerun message.', 'error');
    }
}

export async function deleteMessageHandler(timestamp, role) {
    const targetMessageElement = document.querySelector(`.chat-message[data-timestamp="${timestamp}"]`);

    if (targetMessageElement) {
        if (confirm('Are you sure you want to delete this message?')) {
            const removedFromState = removeMessageByTimestamp(timestamp);
            if (removedFromState) {
                targetMessageElement.remove();
                showToast('Message deleted.', 'success');
                if (state.currentConversationId) {                                 
                    await saveCurrentConversationHandler();
                }
                if (chatHistoryEl && chatHistoryEl.children.length === 0) {
                    displayWelcomeMessage();
                }
            } else {
                showToast('Message not found in history.', 'error');
            }
        }
    } else {
        showToast('Could not find message element to delete.', 'error');
        console.warn('Failed to find message in DOM for deletion, timestamp:', timestamp);
    }
}

export function exportChatToPDFHandler() {
    exportChatToPDF(state.conversationHistory);                 
}

export function handleConversationSearch(event) {
    if (!conversationListEl) return;
    const searchTerm = event.target.value.toLowerCase();
    conversationListEl.querySelectorAll('li').forEach(li => {
        if (li.classList.contains('empty-state')) return;

        const titleEl = li.querySelector('.conversation-title');
        if (titleEl) {
            const title = titleEl.textContent.toLowerCase();
            li.style.display = title.includes(searchTerm) ? '' : 'none';
        } else {
            li.style.display = searchTerm ? 'none' : '';                         
        }
    });
}

export function handleSingleChatSearch(event) {
    if (!chatHistoryEl) return;
    const searchTerm = event.target.value.trim();
    const searchRegex = searchTerm ? new RegExp(escapeRegExp(searchTerm), 'gi') : null;

    chatHistoryEl.querySelectorAll('mark.search-highlight').forEach(mark => {
        const parent = mark.parentNode;
        if (parent) {
            mark.replaceWith(...mark.childNodes);                         
            parent.normalize();                 
        }
    });

    chatHistoryEl.querySelectorAll('.chat-message').forEach(message => {
        message.classList.remove('dimmed');                 
        if (!searchTerm) return;

        const messageContentEl = message.querySelector('.message-content');
        if (!messageContentEl) return;

        if (!messageContentEl.textContent.toLowerCase().includes(searchTerm.toLowerCase())) {
            message.classList.add('dimmed');
            return;
        }

        const treeWalker = document.createTreeWalker(messageContentEl, NodeFilter.SHOW_TEXT, null);
        const textNodesToProcess = [];
        while (treeWalker.nextNode()) {
            const node = treeWalker.currentNode;
            if (node.parentElement.closest('button, script, style, svg, a[href], .chart-controls, .table-controls, .message-controls, .sql-log-actions, .toggle-visibility-btn') || !node.nodeValue.trim()) {
                continue;
            }
            if (searchRegex.test(node.nodeValue)) {
                textNodesToProcess.push(node);
            }
        }

        for (let i = textNodesToProcess.length - 1; i >= 0; i--) {
            const node = textNodesToProcess[i];
            const tempWrapper = document.createElement('span');                         
            const highlightedHTML = node.nodeValue.replace(searchRegex, (match) => `<mark class="search-highlight">${match}</mark>`);
            if (node.nodeValue !== highlightedHTML) {                     
                tempWrapper.innerHTML = highlightedHTML;
                node.replaceWith(...tempWrapper.childNodes);
            }
        }
    });
}

export function handleChatInputKeypress(event) {
    if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        sendMessageHandler();
    }
}

export function autoResizeChatInputHandler() {
    if (!chatInput || chatInput.tagName.toLowerCase() !== 'textarea') return;
    const maxHeight = 200;                 
    chatInput.style.height = 'auto';                     
    let newHeight = chatInput.scrollHeight;

    if (newHeight > maxHeight) {
        newHeight = maxHeight;
        chatInput.style.overflowY = 'auto';
    } else {
        chatInput.style.overflowY = 'hidden';
    }
    chatInput.style.height = newHeight + 'px';
}

export function openSqlEditorHandler(query) {
    openSqlEditor(query);                     
}

