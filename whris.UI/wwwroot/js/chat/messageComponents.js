import { state, updateMessageFeedback, getLatestUserMessageContent } from './state.js';
import { saveCurrentConversationHandler, sendMessageHandler, rerunMessageHandler, deleteMessageHandler } from './eventHandlers.js';
import { showToast } from './toastService.js';
import { copyToClipboard } from './utils.js';
import { chatInput } from './domElements.js';

export function createClarificationButtons(clarificationQuestions) {
    const container = document.createElement('div');
    container.className = 'clarification-container message-section';                     
    clarificationQuestions.forEach(q => {
        const btn = document.createElement('button');
        btn.className = 'suggestion-button';         
        btn.textContent = q;
        btn.onclick = () => {
            const originalQuestion = getLatestUserMessageContent();
            chatInput.value = originalQuestion ? `${originalQuestion} (clarification: ${q})` : q;
            if (typeof sendMessageHandler === 'function') {
                sendMessageHandler();
            } else {
                console.error("sendMessageHandler not available in messageComponents for clarification button.");
            }
        };
        container.appendChild(btn);
    });
    return container;
}

export function createSuggestedQuestions(suggestions) {
    const container = document.createElement('div');
    container.className = 'suggested-questions-container message-section';         
    suggestions.forEach(questionText => {
        const button = document.createElement('button');
        button.className = 'suggestion-button';         
        button.textContent = questionText;
        button.onclick = () => {
            chatInput.value = questionText;
            if (typeof sendMessageHandler === 'function') {
                sendMessageHandler();
            } else {
                console.error("sendMessageHandler not available in messageComponents for suggested question.");
            }
        };
        container.appendChild(button);
    });
    return container;
}

export function createBotActions(messageData) {
    const botActionsContainer = document.createElement('div');
    botActionsContainer.className = 'bot-message-actions';

    const likeBtn = document.createElement('button');
    likeBtn.className = 'bot-message-action-btn like-btn';
    likeBtn.title = 'Like this response';
    likeBtn.innerHTML = `<svg viewBox="0 0 24 24"><path d="M1 21h4V9H1v12zm22-11c0-1.1-.9-2-2-2h-6.31l.95-4.57.03-.32c0-.41-.17-.79-.44-1.06L14.17 1 7.59 7.59C7.22 7.95 7 8.45 7 9v10c0 1.1.9 2 2 2h9c.83 0 1.54-.5 1.84-1.22l3.02-7.05c.09-.23.14-.47.14-.73v-2z"/></svg>`;

    const dislikeBtn = document.createElement('button');
    dislikeBtn.className = 'bot-message-action-btn dislike-btn';
    dislikeBtn.title = 'Dislike this response';
    dislikeBtn.innerHTML = `<svg viewBox="0 0 24 24"><path d="M15 3H6c-.83 0-1.54.5-1.84 1.22l-3.02 7.05c-.09.23-.14.47-.14.73v2c0 1.1.9 2 2 2h6.31l-.95 4.57-.03.32c0 .41.17.79-.44 1.06L9.83 23l6.59-6.59c.36-.36.58-.86.58-1.41V5c0-1.1-.9-2-2-2zm4 0v12h4V3h-4z"/></svg>`;

    const copyBtn = document.createElement('button');
    copyBtn.className = 'bot-message-action-btn copy-btn';
    copyBtn.title = 'Copy text or data';
    copyBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" height="18px" viewBox="0 0 24 24" width="18px" fill="currentColor"><path d="M0 0h24v24H0V0z" fill="none"/><path d="M16 1H4c-1.1 0-2 .9-2 2v14h2V3h12V1zm3 4H8c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h11c1.1 0 2-.9 2-2V7c0-1.1-.9-2-2-2zm0 16H8V7h11v14z"/></svg>`;

    copyBtn.addEventListener('click', () => {
        let textToCopy = "";
        const primaryContent = messageData.content ? messageData.content.trim() : "";
        const insightsContent = messageData.insights ? messageData.insights.trim() : "";

        if (insightsContent) textToCopy += `Insights:\n${insightsContent.replace(/<br\s*\/?>/gi, '\n').replace(/<strong>(.*?)<\/strong>/gi, '$1')}\n\n`;
        if (primaryContent) textToCopy += `Response:\n${primaryContent.replace(/<br\s*\/?>/gi, '\n').replace(/<strong>(.*?)<\/strong>/gi, '$1')}\n\n`;

        const tableDataToCopy = messageData.tableData?.length ? messageData.tableData : (messageData.tableData?.Value?.length ? messageData.tableData.Value : null);
        if (tableDataToCopy && tableDataToCopy.length > 0) {
            let tableString = "Table Data:\n";
            const firstRow = tableDataToCopy[0];
            if (typeof firstRow === 'object' && firstRow !== null) {
                const columnNames = Object.keys(firstRow);
                if (columnNames.length > 0) {
                    tableString += columnNames.join("\t") + "\n";
                    tableDataToCopy.forEach(row => {
                        tableString += columnNames.map(colName => {
                            const cellValue = row[colName];
                            if (cellValue === null || typeof cellValue === 'undefined') return 'NULL';
                            return String(cellValue).replace(/\n/g, ' ').replace(/\t/g, ' ');
                        }).join("\t") + "\n";
                    });
                }
            }
            textToCopy += tableString + "\n";
        }
        if (messageData.sql) textToCopy += `SQL Query:\n${messageData.sql}\n\n`;

        if (textToCopy.trim() === "") {
            showToast("Nothing specific to copy from this message.", "info");
            return;
        }
        copyToClipboard(textToCopy.trim());
    });

    const shareBtn = document.createElement('button');
    shareBtn.className = 'bot-message-action-btn share-btn';
    shareBtn.title = 'Share this response as an image';
    shareBtn.innerHTML = `<svg viewBox="0 0 24 24"><path d="M18 16.08c-.76 0-1.44.3-1.96.77L8.91 12.7c.05-.23.09-.46.09-.7s-.04-.47-.09-.7l7.05-4.11c.54.5 1.25.81 2.04.81 1.66 0 3-1.34 3-3s-1.34-3-3-3-3 1.34-3 3c0 .24.04.47.09.7L8.04 9.81C7.5 9.31 6.79 9 6 9c-1.66 0-3 1.34-3 3s1.34 3 3 3c.79 0 1.5-.31 2.04-.81l7.12 4.16c-.05.21-.08.43-.08.65 0 1.61 1.31 2.92 2.92 2.92s2.92-1.31 2.92-2.92-1.31-2.92-2.92-2.92z"/></svg>`;

    shareBtn.addEventListener('click', async (event) => {
        if (!window.html2canvas) {
            showToast('Share feature requires html2canvas library.', 'error');
            console.error('html2canvas not loaded.');
            return;
        }

        const button = event.currentTarget;
        const originalButtonIcon = button.innerHTML;
        button.innerHTML = '...';
        button.disabled = true;

        const messageBubble = button.closest('.message-bubble');
        if (!messageBubble) {
            showToast('Could not find message to share.', 'error');
            button.disabled = false;
            button.innerHTML = originalButtonIcon;
            return;
        }

        try {
            const canvas = await window.html2canvas(messageBubble, {
                useCORS: true,
                backgroundColor: null,
                scale: 2
            });

            const blob = await new Promise(resolve => canvas.toBlob(resolve, 'image/png'));
            const file = new File([blob], 'ai-assistant-message.png', { type: 'image/png' });

            if (navigator.share && navigator.canShare && navigator.canShare({ files: [file] })) {
                await navigator.share({
                    title: 'AI Assistant Response',
                    text: 'Check out this response I got.',
                    files: [file]
                });
            } else if (navigator.clipboard && navigator.clipboard.write) {
                await navigator.clipboard.write([
                    new ClipboardItem({ 'image/png': blob })
                ]);
                showToast('Screenshot copied to clipboard!', 'success');
            } else {
                const dataUrl = canvas.toDataURL('image/png');
                const link = document.createElement('a');
                link.href = dataUrl;
                link.download = 'ai-assistant-message.png';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                showToast('Screenshot downloaded.', 'info');
            }
        } catch (error) {
            if (error.name !== 'AbortError') {
                console.error('Sharing failed:', error);
                showToast('Sorry, sharing failed.', 'error');
            }
        } finally {
            button.disabled = false;
            button.innerHTML = originalButtonIcon;
            }
    });

    if (messageData.feedback === 'liked') likeBtn.classList.add('liked');
    if (messageData.feedback === 'disliked') dislikeBtn.classList.add('disliked');

    likeBtn.addEventListener('click', async () => {
        const isCurrentlyLiked = likeBtn.classList.contains('liked');
        const newFeedback = isCurrentlyLiked ? null : 'liked';
        likeBtn.classList.toggle('liked', !isCurrentlyLiked);
        dislikeBtn.classList.remove('disliked');

        updateMessageFeedback(messageData.timestamp, newFeedback);
        if (state.currentConversationId && typeof saveCurrentConversationHandler === 'function') {
            await saveCurrentConversationHandler();
        } else if (state.currentConversationId) {
            console.warn("saveCurrentConversationHandler not available for like action.");
        }
    });

    dislikeBtn.addEventListener('click', async () => {
        const isCurrentlyDisliked = dislikeBtn.classList.contains('disliked');
        const newFeedback = isCurrentlyDisliked ? null : 'disliked';

        dislikeBtn.classList.toggle('disliked', !isCurrentlyDisliked);
        likeBtn.classList.remove('liked');

        updateMessageFeedback(messageData.timestamp, newFeedback);
        if (state.currentConversationId && typeof saveCurrentConversationHandler === 'function') {
            await saveCurrentConversationHandler();
        } else if (state.currentConversationId) {
            console.warn("saveCurrentConversationHandler not available for dislike action.");
        }
    });

    const deleteMsgBtn = document.createElement('button');
    deleteMsgBtn.className = 'bot-message-action-btn delete-msg-btn';
    deleteMsgBtn.title = 'Delete this response';
    deleteMsgBtn.innerHTML = `<svg viewBox="0 0 24 24" fill="currentColor"><path d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"/></svg>`;
    deleteMsgBtn.onclick = () => deleteMessageHandler(messageData.timestamp, 'model');


    botActionsContainer.appendChild(likeBtn);
    botActionsContainer.appendChild(dislikeBtn);
    botActionsContainer.appendChild(copyBtn);
    botActionsContainer.appendChild(shareBtn);
    botActionsContainer.appendChild(deleteMsgBtn);             
    return botActionsContainer;
}

export function createUserMessageActions(messageData) {
    const actionsContainer = document.createElement('div');
    actionsContainer.className = 'user-message-actions message-section';                     
    actionsContainer.style.marginTop = '5px';
    actionsContainer.style.paddingTop = '5px';
    actionsContainer.style.alignSelf = 'flex-end';


    const rerunBtn = document.createElement('button');
    rerunBtn.className = 'message-action-btn rerun-btn';
    rerunBtn.title = 'Rerun this message';
    rerunBtn.innerHTML = `<svg viewBox="0 0 24 24" fill="currentColor"><path d="M12 5V1L7 6l5 5V7c3.31 0 6 2.69 6 6s-2.69 6-6 6-6-2.69-6-6H4c0 4.42 3.58 8 8 8s8-3.58 8-8-3.58-8-8-8z"/></svg>`;
    rerunBtn.onclick = () => rerunMessageHandler(messageData.content);
    actionsContainer.appendChild(rerunBtn);

    const deleteBtn = document.createElement('button');
    deleteBtn.className = 'message-action-btn delete-msg-btn';
    deleteBtn.title = 'Delete this message';
    deleteBtn.innerHTML = `<svg viewBox="0 0 24 24" fill="currentColor"><path d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"/></svg>`;
    deleteBtn.onclick = () => deleteMessageHandler(messageData.timestamp, 'user');
    actionsContainer.appendChild(deleteBtn);

    return actionsContainer;
}

export function createSummarizeButton(tableDataToSummarize, summarizeDataHandler) {
    const summarizeBtn = document.createElement('button');
    summarizeBtn.textContent = 'Summarize Data';
    summarizeBtn.className = 'message-control-button';         
    summarizeBtn.style.marginTop = '10px';
    summarizeBtn.style.marginLeft = '10px';
    summarizeBtn.onclick = () => {
        if (typeof summarizeDataHandler === 'function') {
            summarizeDataHandler(tableDataToSummarize);
        } else {
            console.error("summarizeDataHandler not available in messageComponents.");
        }
    };
    return summarizeBtn;
}

export function createDataToggle(collapsibleContent, isInitiallyCollapsed = true) {
    const toggleButton = document.createElement('button');
    toggleButton.className = 'toggle-visibility-btn';         

    collapsibleContent.style.display = isInitiallyCollapsed ? 'none' : 'block';
    toggleButton.innerHTML = isInitiallyCollapsed ?
        `Show Details <svg viewBox="0 0 24 24" class="toggle-icon"><path d="M7 10l5 5 5-5z"/></svg>` :
        `Hide Details <svg viewBox="0 0 24 24" class="toggle-icon"><path d="M7 14l5-5 5 5z"/></svg>`;

    toggleButton.onclick = () => {
        const isCurrentlyCollapsed = collapsibleContent.style.display === 'none';
        collapsibleContent.style.display = isCurrentlyCollapsed ? 'block' : 'none';
        toggleButton.innerHTML = isCurrentlyCollapsed ?
            `Hide Details <svg viewBox="0 0 24 24" class="toggle-icon"><path d="M7 14l5-5 5 5z"/></svg>` :
            `Show Details <svg viewBox="0 0 24 24" class="toggle-icon"><path d="M7 10l5 5 5-5z"/></svg>`;
    };
    return toggleButton;
}

export function createSuggestedVisualizations(suggestedVisualizations, tableDataForMessage, chartDataForMessage) {
    if (!suggestedVisualizations || suggestedVisualizations.length === 0 || (!tableDataForMessage && !chartDataForMessage)) {
        return null;
    }

    const vizContainer = document.createElement('div');
    vizContainer.className = 'suggested-visualizations message-section';         
    vizContainer.innerHTML = '<strong>Suggested Charts:</strong> ';

    let suggestionsAdded = 0;
    suggestedVisualizations.forEach(vizType => {
        if (['bar', 'line', 'pie', 'area', 'scatter', 'doughnut'].includes(vizType.toLowerCase())) {
            const btn = document.createElement('button');
            btn.className = 'control-button suggestion-chip';         
            btn.textContent = vizType.charAt(0).toUpperCase() + vizType.slice(1);
            btn.onclick = () => {
                chatInput.value = `Generate a ${vizType} chart for the previous data`;
                if (typeof sendMessageHandler === 'function') {
                    sendMessageHandler();
                } else {
                    console.error("sendMessageHandler not available for suggested viz button.");
                }
            };
            vizContainer.appendChild(btn);
            suggestionsAdded++;
        }
    });

    return suggestionsAdded > 0 ? vizContainer : null;
}
