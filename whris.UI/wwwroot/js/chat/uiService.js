import { chatHistoryEl, chatInput, sendButton, conversationListEl } from './domElements.js';
import { formatTimestamp, getRandomPrompts, exportD3ChartAsPNG, exportMermaidDiagramAsImage } from './utils.js';
import { allExamplePrompts } from './config.js';
import { state, setCurrentConversationId as updateGlobalCurrentConversationId, addMessageToHistory, findMessageByTimestamp } from './state.js';
import { sendMessageHandler, selectConversationHandler, deleteConversationHandler, togglePinConversationHandler, enableTitleEditHandler, summarizeDataHandler, saveCurrentConversationHandler } from './eventHandlers.js';                                                     

import { renderBarChart, renderLineChart, renderPieChart, renderAreaChart, renderScatterChart, renderDoughnutChart } from './chartRenderer.js';
import { renderTable, renderTreeView } from './tableRenderer.js';
import { renderMap } from './mapRenderer.js';
import { createSqlLog } from './sqlLogRenderer.js';
import { createClarificationButtons, createSuggestedQuestions, createBotActions, createSummarizeButton, createDataToggle, createSuggestedVisualizations, createUserMessageActions } from './messageComponents.js';

export function appendMessage(sender, data) {
    if (!chatHistoryEl) {
        console.error("chatHistoryEl is not defined or not found in DOM!");
        return;
    }

    const emptyState = chatHistoryEl.querySelector('.empty-state');
    if (emptyState) {
        emptyState.remove();
    }

    const messageDiv = document.createElement('div');
    messageDiv.classList.add('chat-message', `${sender}-message`);
    messageDiv.dataset.timestamp = data.timestamp;             

    const avatar = document.createElement('div');
    avatar.className = 'message-avatar';
    avatar.innerHTML = sender === 'user' ?
        '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/></svg>' :
        '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M12 0L15 9L24 12L15 15L12 24L9 15L0 12L9 9Z"/></svg>';
    messageDiv.appendChild(avatar);

    const messageBubble = document.createElement('div');
    messageBubble.className = 'message-bubble';

    const messageContentWrapper = document.createElement('div');
    messageContentWrapper.className = 'message-content';

    let contentRendered = false;

    if (sender === 'user') {
        const p = document.createElement('p');
        p.textContent = data.content;
        messageContentWrapper.appendChild(p);
        messageBubble.appendChild(createUserMessageActions(data));                                 
        contentRendered = true;
    } else {         
        if (data.content) {
            const contentDiv = document.createElement('div');
            contentDiv.className = 'bot-reply-text';
            contentDiv.innerHTML = data.content
                .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
                .replace(/\n/g, '<br/>');
            messageContentWrapper.appendChild(contentDiv);
            contentRendered = true;
        }

        if (data.clarificationQuestions && data.clarificationQuestions.length > 0) {
            messageContentWrapper.appendChild(createClarificationButtons(data.clarificationQuestions));
            contentRendered = true;
        }

        const dataPayloadContainer = document.createElement('div');
        dataPayloadContainer.className = 'data-payload-container message-section';
        let hasDataToCollapse = false;
        let hasInsightsOnly = false;

        const tableDataForMessage = data.tableData?.length ? data.tableData : (data.tableData?.Value?.length ? data.tableData.Value : null);
        const chartDataForMessage = data.chartData?.length ? data.chartData : (data.chartData?.Value?.length ? data.chartData.Value : null);
        const treeDataForMessage = data.treeData?.length ? data.treeData : (data.treeData?.Value?.length ? data.treeData.Value : null);

        if (data.insights) {
            const insightsDiv = document.createElement('div');
            insightsDiv.className = 'insights-container';
            insightsDiv.innerHTML = `<h4><svg viewBox="0 0 24 24" class="section-icon"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z"/></svg>AI Insights</h4>${data.insights.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>').replace(/\n/g, '<br/>')}`;
            dataPayloadContainer.appendChild(insightsDiv);
            hasDataToCollapse = true;
            if (!(chartDataForMessage || tableDataForMessage || treeDataForMessage)) {
                hasInsightsOnly = true;
            }
        }

        if (tableDataForMessage && tableDataForMessage.length > 0 && !data.isCrosstab) {
            dataPayloadContainer.appendChild(createSummarizeButton(tableDataForMessage, summarizeDataHandler));
        }

        const collapsibleContent = document.createElement('div');
        collapsibleContent.className = 'collapsible-content';

        if (chartDataForMessage && chartDataForMessage.length > 0 && !data.isCrosstab) {
            const chartContainer = document.createElement('div');
            chartContainer.className = 'chart-container-render';
            const renderFunctions = { 'bar': renderBarChart, 'line': renderLineChart, 'pie': renderPieChart, 'area': renderAreaChart, 'scatter': renderScatterChart, 'doughnut': renderDoughnutChart };
            let chartTypeToRender = data.chartType || 'bar';
            if (!renderFunctions[chartTypeToRender]) {
                console.warn("Unsupported chart type or render function missing:", chartTypeToRender, ". Defaulting to bar chart.");
                chartTypeToRender = 'bar';
            }
            renderFunctions[chartTypeToRender](chartDataForMessage, chartContainer);
            collapsibleContent.appendChild(chartContainer);
            const chartControls = document.createElement('div');
            chartControls.className = 'chart-controls';
            ['bar', 'line', 'pie', 'area', 'scatter', 'doughnut'].forEach(type => {
                const button = document.createElement('button');
                button.textContent = `${type.charAt(0).toUpperCase() + type.slice(1)}`;
                button.className = 'control-button chart-type-btn';
                if (type === chartTypeToRender) button.classList.add('active');
                button.onclick = () => {
                    if (renderFunctions[type]) {
                        renderFunctions[type](chartDataForMessage, chartContainer);
                        chartControls.querySelector('.active')?.classList.remove('active');
                        button.classList.add('active');
                        const currentBotMessage = findMessageByTimestamp(data.timestamp);
                        if (currentBotMessage) currentBotMessage.chartType = type;
                    }
                };
                chartControls.appendChild(button);
            });
            const pdfExportButton = document.createElement('button');
            pdfExportButton.textContent = 'Export Chart';
            pdfExportButton.className = 'control-button pdf-export-button small-btn';
            pdfExportButton.onclick = () => {
                const now = new Date();
                const timestampSuffix = `${now.getFullYear()}${(now.getMonth() + 1).toString().padStart(2, '0')}${now.getDate().toString().padStart(2, '0')}_${now.getHours().toString().padStart(2, '0')}${now.getMinutes().toString().padStart(2, '0')}`;
                exportD3ChartAsPNG(chartContainer, `Chart_${timestampSuffix}.png`);                             
            };
            chartControls.appendChild(pdfExportButton);
            collapsibleContent.appendChild(chartControls);
            hasDataToCollapse = true;
            contentRendered = true;
        }

        if (tableDataForMessage && tableDataForMessage.length > 0) {
            const tableContainer = document.createElement('div');
            if (data.isCrosstab && !data.isPivot) {
                tableContainer.className = 'pivot-table-container-render';
                renderTable(tableDataForMessage, tableContainer, true, false);
            }
            else if (data.isCrosstab && data.isPivot)
            {
                tableContainer.className = 'pivot-table-container-render';
                renderTable(tableDataForMessage, tableContainer, true, true);
            }            
            else
            {
                tableContainer.className = 'table-container-render';
                renderTable(tableDataForMessage, tableContainer, false, false);
            }
            collapsibleContent.appendChild(tableContainer);
            hasDataToCollapse = true;
            contentRendered = true;
        }
        else if (data.isTree && data.treeData && data.treeData.length > 0) {
            const treeContainer = document.createElement('div');
            treeContainer.className = 'tree-view-render-container message-section';
            if (typeof renderTreeView === 'function') {
                renderTreeView(data.treeData, treeContainer, data.aiTreeConfig);
            } else {
                console.error("renderTreeView function is not available.");
                treeContainer.textContent = "[Tree view rendering function missing]";
            }
            collapsibleContent.appendChild(treeContainer);
            contentRendered = true;
        }

        if (hasDataToCollapse) {
            const initiallyCollapsed = !hasInsightsOnly;
            dataPayloadContainer.appendChild(createDataToggle(collapsibleContent, initiallyCollapsed));
        }

        dataPayloadContainer.appendChild(collapsibleContent);

        if (data.insights || (chartDataForMessage && chartDataForMessage.length > 0) || (tableDataForMessage && tableDataForMessage.length > 0)) {
            messageContentWrapper.appendChild(dataPayloadContainer);
        }

        if (data.mapLatitude && data.mapLongitude) {
            renderMap(data, messageContentWrapper);
            contentRendered = true;
        } else if (data.mapQuery && typeof data.mapQuery === 'string' && data.mapQuery.startsWith('http')) {
            renderMap(data, messageContentWrapper);
            contentRendered = true;
        }

        if (data.mermaidDiagramSyntax) {
            const mermaidOuterContainer = document.createElement('div');
            mermaidOuterContainer.className = 'mermaid-exportable-container message-section';

            const diagramOutputDiv = document.createElement('div');
            diagramOutputDiv.className = 'mermaid-diagram-output';
            mermaidOuterContainer.appendChild(diagramOutputDiv);

            const syntaxPre = document.createElement('pre');
            syntaxPre.style.whiteSpace = 'pre-wrap';
            syntaxPre.style.backgroundColor = '#f0f0f0';
            syntaxPre.style.padding = '10px';
            syntaxPre.style.marginTop = '10px';
            syntaxPre.style.border = '1px dashed #ccc';
            syntaxPre.style.display = 'none';
            syntaxPre.textContent = `Attempted Mermaid Syntax:\n\n${data.mermaidDiagramSyntax}`;
            mermaidOuterContainer.appendChild(syntaxPre);

            messageContentWrapper.appendChild(mermaidOuterContainer);

            if (window.mermaid && typeof window.mermaid.render === 'function') {
                setTimeout(async () => {
                    let renderedSvgElement = null;
                    try {
                        const diagramId = `mermaid-diag-${Date.now()}-${Math.random().toString(36).substr(2, 5)}`;
                        const result = await window.mermaid.render(diagramId, data.mermaidDiagramSyntax);

                        diagramOutputDiv.innerHTML = result.svg;
                        if (result.bindFunctions) {
                            result.bindFunctions(diagramOutputDiv);
                        }
                        renderedSvgElement = diagramOutputDiv.querySelector('svg');

                        if (renderedSvgElement && (!result.svg.includes("Syntax error in text") && !result.svg.includes("mermaidAPI.parseError"))) {
                            syntaxPre.style.display = 'none';

                            const exportButton = document.createElement('button');
                            exportButton.textContent = 'Export Diagram';
                            exportButton.className = 'control-button export-diagram-btn';
                            exportButton.style.marginTop = '10px';
                            exportButton.style.display = 'block';
                            exportButton.style.marginLeft = 'auto';
                            exportButton.style.marginRight = 'auto';
                            exportButton.title = 'Export diagram as PNG';

                            exportButton.onclick = () => {
                                if (typeof exportMermaidDiagramAsImage === 'function' && diagramOutputDiv.firstChild) {
                                    const now = new Date();
                                    const timestampSuffix = `${now.getFullYear()}${(now.getMonth() + 1).toString().padStart(2, '0')}${now.getDate().toString().padStart(2, '0')}_${now.getHours().toString().padStart(2, '0')}${now.getMinutes().toString().padStart(2, '0')}`;
                                    exportMermaidDiagramAsImage(diagramOutputDiv, `Diagram_${timestampSuffix}.png`);
                                } else {
                                    console.error("exportMermaidDiagramAsImage function not found or diagram element for export is missing.");
                                    showToast("Failed to initiate diagram export.", "error");
                                }
                            };
                            mermaidOuterContainer.appendChild(exportButton);
                        } else {
                            syntaxPre.style.display = 'block';
                            if ((data.mermaidDiagramSyntax || "").toUpperCase() === "ERROR" && !diagramOutputDiv.innerHTML.includes("Syntax error")) {
                                diagramOutputDiv.innerHTML = `<p style='color:red; font-weight:bold;'>The AI indicated it could not generate a diagram for this request.</p>`;
                            }
                            console.warn("Mermaid rendering reported an error, or AI returned an error string.");
                        }
                    } catch (e) {
                        console.error("Exception during mermaid.render() or button setup:", e);
                        syntaxPre.style.display = 'block';
                        diagramOutputDiv.innerHTML = `<p style='color:red; font-weight:bold;'>Diagram Display Failed (Exception)</p>`;
                        if (e.message) {
                            const errMsgP = document.createElement('p');
                            errMsgP.style.color = 'red';
                            errMsgP.textContent = `Details: ${e.message.substring(0, 100)}...`;
                            diagramOutputDiv.appendChild(errMsgP);
                        }
                    }
                }, 50);
            } else {
                console.warn("Mermaid.js or mermaid.render() not available. Diagram will show as raw text.");
                diagramOutputDiv.innerHTML = `<p style='color:orange; font-weight:bold;'>Mermaid rendering library not available.</p>`;
                syntaxPre.style.display = 'block';
            }
            contentRendered = true;
        }

        else if (tableDataForMessage && tableDataForMessage.length > 0)
        {
            if (!data.isCrosstab && !(chartDataForMessage && chartDataForMessage.length > 0)) {
                const suggestedVizElem = createSuggestedVisualizations(data.suggestedVisualizations, tableDataForMessage, chartDataForMessage);
                if (suggestedVizElem) {
                    messageContentWrapper.appendChild(suggestedVizElem);
                }
            }
        }       

        if (data.executedQueries && data.executedQueries.length > 0) {
            messageContentWrapper.appendChild(createSqlLog(data.executedQueries));
            contentRendered = true;
        }

        const suggestions = data.suggestedQuestions || data.SuggestedQuestions;
        if (suggestions && suggestions.length > 0) {
            messageContentWrapper.appendChild(createSuggestedQuestions(suggestions));
        }

        if (contentRendered) {
            messageBubble.appendChild(createBotActions(data));             
        }
    }

    const senderNameDiv = document.createElement('div');
    senderNameDiv.className = 'message-sender-name';
    senderNameDiv.textContent = sender === 'user' ? 'You' : 'AI Assistant';

    const timestampEl = document.createElement('span');
    timestampEl.className = 'user-message-timestamp';
    timestampEl.textContent = data.timestamp ? formatTimestamp(data.timestamp) : '';

    const senderInfoDiv = document.createElement('div');
    senderInfoDiv.className = 'message-sender-info';
    senderInfoDiv.appendChild(senderNameDiv);
    senderInfoDiv.appendChild(timestampEl);

    messageBubble.appendChild(senderInfoDiv);
    messageBubble.appendChild(messageContentWrapper);                                 

    messageDiv.appendChild(messageBubble);

    chatHistoryEl.appendChild(messageDiv);
    chatHistoryEl.scrollTop = chatHistoryEl.scrollHeight;
}

export function displayWelcomeMessage() {
    if (!chatHistoryEl) return;
    chatHistoryEl.innerHTML = '';

    const numPromptsToShow = 3;
    const selectedPrompts = getRandomPrompts(allExamplePrompts, numPromptsToShow);

    let promptsHtml = '';
    selectedPrompts.forEach(promptText => {
        promptsHtml += `<div class="example-prompt">${promptText}</div>`;
    });

    const welcomeDiv = document.createElement('div');
    welcomeDiv.className = 'empty-state';
    welcomeDiv.innerHTML = `
        <div class="empty-state-icon">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                <path d="M12 0 L15 9 L24 12 L15 15 L12 24 L9 15 L0 12 L9 9 Z" />
            </svg>
        </div>
        <h2>Hi, I'm your AI Assistant!</h2>
        <p>I can help you query your database. Try one of these examples or ask your own question:</p>
        <div class="example-prompts-container">
            ${promptsHtml}
        </div>
    `;
    chatHistoryEl.appendChild(welcomeDiv);

    const examplePromptDivs = welcomeDiv.querySelectorAll('.example-prompt');
    examplePromptDivs.forEach(promptDiv => {
        promptDiv.addEventListener('click', () => {
            const question = promptDiv.textContent;
            if (chatInput) chatInput.value = question;
            if (typeof sendMessageHandler === 'function') {
                sendMessageHandler();
            } else {
                console.error("sendMessageHandler not available for welcome prompt.");
            }
        });
    });
    if (chatInput) chatInput.focus();
}

export function updateConversationList(conversations, currentConvId) {
    if (!conversationListEl) return;
    conversationListEl.innerHTML = '';

    if (conversations.length === 0) {
        conversationListEl.innerHTML = `<li class="empty-state" style="padding: 20px; text-align: center; color: #6c757d;">Your saved conversations will appear here.</li>`;
        return;
    }

    conversations.forEach(conv => {
        const li = document.createElement('li');
        li.dataset.id = conv.Id;
        li.title = conv.Title;
        if (conv.Id === currentConvId) {
            li.classList.add('active');
        }

        const pinBtn = document.createElement('button');
        pinBtn.className = `pin-conversation-btn ${conv.Pin ? 'pinned' : ''}`;
        pinBtn.title = conv.Pin ? 'Unpin chat' : 'Pin chat';
        pinBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" height="24px" viewBox="0 0 24 24" width="24px" fill="currentColor"><path d="M0 0h24v24H0V0z" fill="none"/><path d="M16 9V4h2V2H6v2h2v5c0 1.66-1.34 3-3 3v2h5.97v7l1 1 1-1v-7H19v-2c-1.66 0-3-1.34-3-3z"/></svg>`;

        const titleSpan = document.createElement('span');
        titleSpan.className = 'conversation-title';
        titleSpan.textContent = conv.Title;

        const actionsContainer = document.createElement('div');
        actionsContainer.className = 'conversation-actions';

        const editBtn = document.createElement('button');
        editBtn.className = 'edit-conversation-btn';
        editBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" height="18px" viewBox="0 0 24 24" width="18px" fill="currentColor"><path d="M0 0h24v24H0V0z" fill="none"/><path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg>`;
        editBtn.title = 'Edit title';

        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'delete-conversation-btn';
        deleteBtn.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" height="18px" viewBox="0 0 24 24" width="18px" fill="currentColor"><path d="M0 0h24v24H0V0z" fill="none"/><path d="M16 9v10H8V9h8m-1.5-6h-5l-1 1H5v2h14V4h-3.5l-1-1zM18 7H6v12c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7z"/></svg>`;
        deleteBtn.title = 'Delete chat';

        actionsContainer.appendChild(pinBtn);
        actionsContainer.appendChild(editBtn);
        actionsContainer.appendChild(deleteBtn);

        li.appendChild(titleSpan);
        li.appendChild(actionsContainer);

        li.onclick = () => {
            if (typeof selectConversationHandler === 'function') selectConversationHandler(conv.Id);
        };
        pinBtn.onclick = (e) => {
            e.stopPropagation();
            if (typeof togglePinConversationHandler === 'function') togglePinConversationHandler(conv.Id);
        };
        editBtn.onclick = (e) => {
            e.stopPropagation();
            if (typeof enableTitleEditHandler === 'function') enableTitleEditHandler(titleSpan, conv.Id, conv.Title);
        };
        deleteBtn.onclick = (e) => {
            e.stopPropagation();
            if (typeof deleteConversationHandler === 'function') deleteConversationHandler(conv.Id, conv.Title);
        };
        conversationListEl.appendChild(li);
    });
}

export function setSendingState(isSending) {
    if (sendButton) {
        sendButton.disabled = isSending;
        if (isSending) {
            sendButton.innerHTML = '<div class="dot-pulse" style="margin: auto; background-color: #3c8eb3;"></div>';
        } else {
            sendButton.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor"><path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2 .01 7z"></path></svg>`;
        }
    }
    if (chatInput) { chatInput.disabled = isSending; }                         
}

export function showThinkingIndicator() {
    if (!chatHistoryEl) return;
    document.getElementById('thinking-indicator')?.remove();

    const thinkingDiv = document.createElement('div');
    thinkingDiv.id = 'thinking-indicator';
    thinkingDiv.classList.add('chat-message', 'bot-message');
    thinkingDiv.innerHTML = `<div class="message-content"><p><div class="thinking-indicator-content">Thinking <div class="dot-pulse"></div><div class="dot-pulse"></div><div class="dot-pulse"></div></div></p></div>`;
    chatHistoryEl.appendChild(thinkingDiv);
    chatHistoryEl.scrollTop = chatHistoryEl.scrollHeight;
}

export function removeThinkingIndicator() {
    document.getElementById('thinking-indicator')?.remove();
}

export function showSummaryThinkingIndicator() {
    if (!chatHistoryEl) return;
    document.querySelector('.summary-thinking')?.remove();

    const thinkingDiv = document.createElement('div');
    thinkingDiv.classList.add('chat-message', 'bot-message', 'summary-thinking');
    thinkingDiv.innerHTML = `<div class="message-content"><p><div class="thinking-indicator-content">Thinking <div class="dot-pulse"></div><div class="dot-pulse"></div><div class="dot-pulse"></div></div></p></div>`;
    chatHistoryEl.appendChild(thinkingDiv);
    chatHistoryEl.scrollTop = chatHistoryEl.scrollHeight;
}

export function removeSummaryThinkingIndicator() {
    document.querySelector('.summary-thinking')?.remove();
}

export function clearChatUI() {
    if (chatHistoryEl) chatHistoryEl.innerHTML = '';
    document.querySelectorAll('#conversation-list li.active').forEach(el => el.classList.remove('active'));
}

export function markConversationActive(conversationId) {
    document.querySelectorAll('#conversation-list li.active').forEach(el => el.classList.remove('active'));
    const currentLi = document.querySelector(`#conversation-list li[data-id='${conversationId}']`);
    if (currentLi) {
        currentLi.classList.add('active');
    } else {
        console.warn(`[UI_SERVICE] Could not find list item for conversation ID ${conversationId} to mark as active.`);
    }
}

export function updateConversationListItemTitle(conversationId, newTitle) {
    const listItem = conversationListEl.querySelector(`li[data-id='${conversationId}'] .conversation-title`);
    if (listItem && newTitle && listItem.textContent !== newTitle) {
        listItem.textContent = newTitle;
        listItem.title = newTitle;
    }
}