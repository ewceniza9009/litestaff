import { exportDataToCSV } from './utils.js';

let tableSortState = {};
let currentPivotViewState = 'pivot';

export function renderTable(data, container, isCrosstabOrPivot = false, isPivot = false) {
    console.log('[RenderTable] Initializing. isCrosstabOrPivot:', isCrosstabOrPivot, 'isPivot:', isPivot, 'Data rows:', data ? data.length : 0);
    if (data && data.length > 0 && data[0]) {
        try {
            console.log('[RenderTable] Sample data row:', JSON.parse(JSON.stringify(data[0])));
        } catch (e) {
            console.warn('[RenderTable] Could not stringify sample data row for logging:', e);
        }
    }

    container.innerHTML = '';
    if (!data || data.length === 0) {
        container.textContent = 'No data to display.';
        return;
    }

    const originalColumnNames = (data[0] && typeof data[0] === 'object') ? Object.keys(data[0]) : [];
    console.log('[RenderTable] Original Column Names:', originalColumnNames);

    let groupKeyField = null;
    let detailKeyField = null;
    let pivotValueFields = [];
    let displayHeaders = [];
    let trueTableType = 'standard';
    let pivotViewToggleDisabled = false;

    if (isCrosstabOrPivot) {
        if (originalColumnNames.length === 0) {
            trueTableType = 'flat_crosstab_no_group_key';
            currentPivotViewState = 'crosstab';
            pivotViewToggleDisabled = true;
            displayHeaders = [];
        } else {
            groupKeyField = originalColumnNames[0];

            if (isPivot) {
                if (originalColumnNames.length >= 2) {
                    trueTableType = 'detail_pivot';
                    currentPivotViewState = 'pivot';
                    detailKeyField = originalColumnNames[1];
                    pivotValueFields = originalColumnNames.length > 2 ? originalColumnNames.slice(2) : [];
                    displayHeaders = originalColumnNames.length > 2 ? [groupKeyField, detailKeyField, ...pivotValueFields] : [groupKeyField, detailKeyField];
                    pivotViewToggleDisabled = false;
                } else {
                    console.warn('[RenderTable] isPivot=true but < 2 columns. Treating as summary_crosstab.');
                    trueTableType = 'summary_crosstab';
                    currentPivotViewState = 'crosstab';
                    detailKeyField = null;
                    pivotValueFields = [];
                    displayHeaders = [groupKeyField];
                    pivotViewToggleDisabled = true;
                }
            } else {
                trueTableType = 'summary_crosstab';
                currentPivotViewState = 'crosstab';
                detailKeyField = null;
                pivotValueFields = originalColumnNames.length > 1 ? originalColumnNames.slice(1) : [];
                displayHeaders = originalColumnNames.length > 1 ? [groupKeyField, ...pivotValueFields] : [groupKeyField];
                pivotViewToggleDisabled = true;
            }
            console.log(`[RenderTable] Classified as ${trueTableType}. GroupKey: ${groupKeyField}, DetailKey: ${detailKeyField}, Values:`, pivotValueFields, "Default View:", currentPivotViewState);
        }
    } else {
        trueTableType = 'standard';
        displayHeaders = [...originalColumnNames];
    }

    const table = document.createElement('table');
    table.className = 'results-table';
    if (isCrosstabOrPivot) table.classList.add('crosstab-table');

    const thead = document.createElement('thead');
    const headerRow = document.createElement('tr');
    thead.appendChild(headerRow); table.appendChild(thead);

    const tbody = document.createElement('tbody');
    table.appendChild(tbody); container.appendChild(table);

    const controlsContainer = document.createElement('div');
    controlsContainer.className = 'table-controls-wrapper';
    const topControls = document.createElement('div');
    topControls.className = 'table-controls table-top-controls';
    controlsContainer.appendChild(topControls);

    const searchContainer = document.createElement('div'); searchContainer.className = 'table-search-container';
    const searchInput = document.createElement('input'); searchInput.type = 'text'; searchInput.placeholder = 'Search table...'; searchInput.className = 'table-search-input';
    searchContainer.appendChild(searchInput); topControls.appendChild(searchContainer);

    const rightControls = document.createElement('div'); rightControls.className = 'table-right-controls';
    const exportButton = document.createElement('button'); exportButton.textContent = 'Export to CSV'; exportButton.className = 'control-button export-button';
    rightControls.appendChild(exportButton); topControls.appendChild(rightControls);
    container.insertBefore(controlsContainer, table);

    let bottomControlsContainer = null;
    if (trueTableType === 'detail_pivot' || trueTableType === 'summary_crosstab') {
        bottomControlsContainer = document.createElement('div');
        bottomControlsContainer.className = 'table-view-toggle-controls';

        const pivotViewButton = document.createElement('button');
        pivotViewButton.textContent = 'Pivot View (Expand/Collapse)';
        pivotViewButton.className = 'control-button view-toggle-btn';
        pivotViewButton.disabled = pivotViewToggleDisabled;
        pivotViewButton.onclick = () => {
            if (!pivotViewButton.disabled && currentPivotViewState !== 'pivot') {
                currentPivotViewState = 'pivot'; updateTableDisplay();
            }
        };

        const crosstabViewButton = document.createElement('button');
        crosstabViewButton.textContent = 'Crosstab View (All Details)';
        crosstabViewButton.className = 'control-button view-toggle-btn';
        crosstabViewButton.onclick = () => {
            if (currentPivotViewState !== 'crosstab') {
                currentPivotViewState = 'crosstab'; updateTableDisplay();
            }
        };
        bottomControlsContainer.appendChild(pivotViewButton);
        bottomControlsContainer.appendChild(crosstabViewButton);
        container.appendChild(bottomControlsContainer);
    }

    let currentDataForDisplay = [...data];
    const rowsPerPage = 10;
    let currentPage = 1;
    let paginationNavContainer = null;
    let updatePaginationControlsFn = null;

    function renderNonPivotPage(pageDataToRender) {
        tbody.innerHTML = '';
        const start = (currentPage - 1) * rowsPerPage;
        const end = start + rowsPerPage;
        const paginatedItems = pageDataToRender.slice(start, end);
        paginatedItems.forEach(rowData => {
            const row = tbody.insertRow();
            originalColumnNames.forEach(colName => {
                const cell = row.insertCell();
                const cellValue = rowData[colName];
                cell.textContent = (typeof cellValue === 'number' && !isNaN(cellValue)) ? cellValue.toLocaleString() : (cellValue ?? 'NULL');
                if (typeof cellValue === 'number' && !isNaN(cellValue)) cell.classList.add('numeric-cell');
            });
        });
    }

    function setupNonPivotPagination(totalItemsCount) {
        if (paginationNavContainer) paginationNavContainer.remove();
        paginationNavContainer = null;
        if (totalItemsCount <= rowsPerPage) {
            if (rightControls && rightControls.querySelector('.pagination-nav-container')) {
                rightControls.querySelector('.pagination-nav-container').remove();
            }
            return null;
        }
        paginationNavContainer = document.createElement('div');
        paginationNavContainer.className = 'pagination-nav-container';
        const prevButton = document.createElement('button'); prevButton.textContent = 'Previous'; prevButton.className = 'control-button';
        prevButton.onclick = () => { if (currentPage > 1) { currentPage--; updateTableDisplay(); } };
        const nextButton = document.createElement('button'); nextButton.textContent = 'Next'; nextButton.className = 'control-button';
        nextButton.onclick = () => { if (currentPage < Math.ceil(totalItemsCount / rowsPerPage)) { currentPage++; updateTableDisplay(); } };
        const pageInfo = document.createElement('span'); pageInfo.className = 'table-pagination-info';
        paginationNavContainer.appendChild(pageInfo);
        paginationNavContainer.appendChild(prevButton);
        paginationNavContainer.appendChild(nextButton);
        if (rightControls && exportButton) rightControls.insertBefore(paginationNavContainer, exportButton);
        else if (rightControls) rightControls.appendChild(paginationNavContainer);
        const updateControls = () => {
            const totalPages = Math.ceil(totalItemsCount / rowsPerPage);
            pageInfo.textContent = `Page ${currentPage} of ${totalPages}`;
            prevButton.disabled = currentPage === 1;
            nextButton.disabled = currentPage === totalPages || totalPages === 0;
            if (paginationNavContainer) paginationNavContainer.style.display = totalPages > 1 ? 'flex' : 'none';
        };
        updateControls(); return updateControls;
    }

    function renderPivotOrCrosstabView(sourceDataToRender) {
        tbody.innerHTML = '';
        console.log(`[RenderPivotOrCrosstab] Mode: ${currentPivotViewState}, GroupKey: ${groupKeyField}, DetailKey: ${detailKeyField}, Items: ${sourceDataToRender.length}`);
        if (!groupKeyField || sourceDataToRender.length === 0) {
            const colSpan = displayHeaders.length || 1;
            tbody.innerHTML = `<tr><td colspan="${colSpan}">No data for this view.</td></tr>`;
            return;
        }
        const groupedData = sourceDataToRender.reduce((acc, row) => {
            const groupVal = row[groupKeyField];
            const groupKeyForMap = (groupVal === undefined || groupVal === null || String(groupVal).trim() === '') ? '__undefined_group__' : String(groupVal);
            if (!acc[groupKeyForMap]) acc[groupKeyForMap] = [];
            acc[groupKeyForMap].push(row);
            return acc;
        }, {});
        Object.entries(groupedData).forEach(([currentGroupKey, itemsInGroup]) => {
            const displayGroupValue = currentGroupKey === '__undefined_group__' ? '(Not Specified)' : currentGroupKey;
            const groupHeaderRow = tbody.insertRow(); groupHeaderRow.classList.add('pivot-group-header');
            const safeGroupIdPart = String(displayGroupValue).replace(/[^a-zA-Z0-9_-]/g, '_');
            const safeGroupKeyFieldPart = groupKeyField.replace(/[^a-zA-Z0-9_-]/g, '_');
            const uniqueGroupId = `details-${safeGroupKeyFieldPart}-${safeGroupIdPart}-${Math.random().toString(36).substr(2, 9)}`;
            const groupKeyCell = groupHeaderRow.insertCell(); groupKeyCell.classList.add('pivot-group-key-cell');
            const canShowToggleButton = currentPivotViewState === 'pivot' && detailKeyField && itemsInGroup && itemsInGroup.length > 0;
            if (canShowToggleButton) {
                groupKeyCell.classList.add('pivot-toggle-cell');
                const toggleBtn = document.createElement('button'); toggleBtn.className = 'pivot-toggle-btn'; toggleBtn.textContent = '+';
                toggleBtn.setAttribute('aria-expanded', 'false'); toggleBtn.setAttribute('aria-controls', uniqueGroupId);
                groupKeyCell.appendChild(toggleBtn); groupKeyCell.appendChild(document.createTextNode(` ${displayGroupValue}`));
                toggleBtn.onclick = (e) => {
                    e.stopPropagation();
                    const isExpanded = toggleBtn.getAttribute('aria-expanded') === 'true';
                    document.querySelectorAll(`.${uniqueGroupId}`).forEach(detailRowElem => { detailRowElem.style.display = isExpanded ? 'none' : ''; });
                    toggleBtn.textContent = isExpanded ? '+' : '-'; toggleBtn.setAttribute('aria-expanded', String(!isExpanded));
                };
            } else {
                groupKeyCell.textContent = displayGroupValue;
            }
            if (detailKeyField && displayHeaders.includes(detailKeyField)) groupHeaderRow.insertCell().textContent = '';
            pivotValueFields.forEach(pivotField => {
                const totalCell = groupHeaderRow.insertCell();
                const sum = itemsInGroup.reduce((s, item) => s + (parseFloat(String(item[pivotField]).replace(/,/g, '')) || 0), 0);
                totalCell.textContent = sum.toLocaleString(); totalCell.classList.add('numeric-cell', 'group-total-cell');
            });
            if (detailKeyField && itemsInGroup && itemsInGroup.length > 0) {
                if (currentPivotViewState === 'pivot') {
                    if (canShowToggleButton) {
                        itemsInGroup.forEach((itemData) => {
                            const detailRow = tbody.insertRow(); detailRow.classList.add('pivot-detail-row', uniqueGroupId); detailRow.style.display = 'none';
                            detailRow.insertCell().textContent = '';
                            if (displayHeaders.includes(detailKeyField)) {
                                const detailKeyCell = detailRow.insertCell(); detailKeyCell.textContent = itemData[detailKeyField] ?? 'NULL';
                                detailKeyCell.classList.add('pivot-detail-key-cell'); detailKeyCell.style.paddingLeft = "30px";
                            }
                            pivotValueFields.forEach(pivotField => {
                                const valueCell = detailRow.insertCell(); const val = itemData[pivotField];
                                valueCell.textContent = (typeof val === 'number' && !isNaN(val)) ? val.toLocaleString() : (val ?? 'NULL');
                                if (typeof val === 'number' && !isNaN(val)) valueCell.classList.add('numeric-cell');
                            });
                        });
                    }
                } else {
                    itemsInGroup.forEach((itemData) => {
                        const detailRow = tbody.insertRow(); detailRow.classList.add('pivot-detail-row'); detailRow.style.display = '';
                        const groupRepeatCell = detailRow.insertCell(); groupRepeatCell.textContent = displayGroupValue; groupRepeatCell.classList.add('crosstab-repeated-group-key');
                        if (displayHeaders.includes(detailKeyField)) {
                            const detailKeyCell = detailRow.insertCell(); detailKeyCell.textContent = itemData[detailKeyField] ?? 'NULL';
                            detailKeyCell.classList.add('pivot-detail-key-cell');
                        }
                        pivotValueFields.forEach(pivotField => {
                            const valueCell = detailRow.insertCell(); const val = itemData[pivotField];
                            valueCell.textContent = (typeof val === 'number' && !isNaN(val)) ? val.toLocaleString() : (val ?? 'NULL');
                            if (typeof val === 'number' && !isNaN(val)) valueCell.classList.add('numeric-cell');
                        });
                    });
                }
            }
        });
    }

    function updateHeaderStructure() {
        headerRow.innerHTML = '';
        displayHeaders.forEach(key => {
            const th = document.createElement('th');
            th.textContent = (key === null || key === undefined) ? '' : String(key);
            headerRow.appendChild(th);
        });
    }

    function updateHeaderSortability() {
        const thElements = headerRow.querySelectorAll('th');
        thElements.forEach((th, index) => {
            const key = displayHeaders[index];
            th.classList.remove('sortable', 'crosstab-row-header'); th.removeAttribute('data-column');
            let arrowSpan = th.querySelector('.sort-arrow'); if (arrowSpan) arrowSpan.remove();
            if (key === null || key === undefined) return;
            if ((trueTableType === 'detail_pivot') && key === groupKeyField && currentPivotViewState === 'pivot') {
                th.classList.add('crosstab-row-header');
            } else if (originalColumnNames.includes(key)) {
                th.classList.add('sortable'); th.dataset.column = key;
                arrowSpan = document.createElement('span'); arrowSpan.className = 'sort-arrow';
                if (tableSortState[key]) arrowSpan.textContent = tableSortState[key] === 'asc' ? '▲' : '▼';
                th.appendChild(arrowSpan);
            }
        });
    }

    function updateTableDisplay() {
        console.log('[UpdateTableDisplay] Called. True Table Type:', trueTableType, 'Current View State:', currentPivotViewState);
        if (paginationNavContainer) paginationNavContainer.remove(); paginationNavContainer = null; updatePaginationControlsFn = null;
        updateHeaderStructure(); updateHeaderSortability();
        table.classList.toggle('expandable-pivot', trueTableType === 'detail_pivot' && currentPivotViewState === 'pivot');
        if (bottomControlsContainer) {
            const pivotBtn = bottomControlsContainer.querySelector('button:nth-child(1)');
            const crosstabBtn = bottomControlsContainer.querySelector('button:nth-child(2)');
            if (pivotBtn) { pivotBtn.classList.toggle('active', currentPivotViewState === 'pivot'); pivotBtn.disabled = pivotViewToggleDisabled; }
            if (crosstabBtn) crosstabBtn.classList.toggle('active', currentPivotViewState === 'crosstab');
            bottomControlsContainer.style.display = (trueTableType === 'detail_pivot' || trueTableType === 'summary_crosstab') ? 'flex' : 'none';
        }
        if (trueTableType === 'standard') {
            renderNonPivotPage(currentDataForDisplay);
            updatePaginationControlsFn = setupNonPivotPagination(currentDataForDisplay.length);
        } else {
            renderPivotOrCrosstabView(currentDataForDisplay);
        }
    }

    exportButton.onclick = () => {
        const now = new Date();
        const timestamp = `${now.getFullYear()}${(now.getMonth() + 1).toString().padStart(2, '0')}${now.getDate().toString().padStart(2, '0')}_${now.getHours().toString().padStart(2, '0')}${now.getMinutes().toString().padStart(2, '0')}`;
        exportDataToCSV(currentDataForDisplay, originalColumnNames, `Export_${timestamp}.csv`);
    };
    searchInput.addEventListener('input', (e) => {
        const searchTerm = e.target.value.toLowerCase();
        currentDataForDisplay = data.filter(row => originalColumnNames.some(colName => {
            const cellValue = row[colName];
            return cellValue !== null && cellValue !== undefined && String(cellValue).toLowerCase().includes(searchTerm);
        }));
        currentPage = 1; tableSortState = {}; updateTableDisplay();
    });
    thead.removeEventListener('click', handleSortClick); thead.addEventListener('click', handleSortClick);
    function handleSortClick(event) {
        let th = event.target; while (th && th.tagName !== 'TH') th = th.parentElement;
        if (!th || !th.classList.contains('sortable')) return;
        const column = th.dataset.column;
        if (!column || !originalColumnNames.includes(column)) { console.warn(`[Sort] Invalid column: ${column}`); return; }
        const currentSortDirection = tableSortState[column]; const direction = (currentSortDirection === 'asc') ? 'desc' : 'asc';
        headerRow.querySelectorAll('.sort-arrow').forEach(arrow => arrow.textContent = '');
        const arrowSpan = th.querySelector('.sort-arrow'); if (arrowSpan) arrowSpan.textContent = direction === 'asc' ? '▲' : '▼';
        tableSortState = { [column]: direction };
        currentDataForDisplay.sort((a, b) => {
            const valA = a[column]; const valB = b[column];
            if (valA === null || typeof valA === 'undefined') return direction === 'asc' ? 1 : -1;
            if (valB === null || typeof valB === 'undefined') return direction === 'asc' ? -1 : 1;
            const numA = parseFloat(String(valA).replace(/,/g, '')); const numB = parseFloat(String(valB).replace(/,/g, ''));
            if (!isNaN(numA) && !isNaN(numB)) return direction === 'asc' ? numA - numB : numB - numA;
            const looksLikeDate = (str) => typeof str === 'string' && (str.match(/^\d{1,2}\/\d{1,2}\/\d{4}/) || str.match(/^\d{4}-\d{1,2}-\d{1,2}/) || str.match(/^\d{4}\/\d{1,2}\/\d{1,2}/));
            if (looksLikeDate(valA) && looksLikeDate(valB)) {
                try {
                    const dateA = new Date(valA); const dateB = new Date(valB);
                    if (dateA instanceof Date && !isNaN(dateA) && dateB instanceof Date && !isNaN(dateB)) return direction === 'asc' ? dateA - dateB : dateB - dateA;
                } catch (e) { }
            }
            return String(valA).localeCompare(String(valB), undefined, { numeric: true, sensitivity: 'base' }) * (direction === 'asc' ? 1 : -1);
        });
        currentPage = 1; updateTableDisplay();
    }

    updateTableDisplay();
}

function SanitizeKeyForJsonJS(key) {
    if (!key || typeof key !== 'string') return "unknown_key_js_sanitized";
    return key.replace(/\s+/g, '_').replace(/[^a-zA-Z0-9_]/g, '');
}

function getTreeLineSVG(segmentType, config = {}) {
    const segmentWidth = config.width || 18;
    const segmentHeight = config.height || 22;
    const lineColor = config.color || '#4a4a4a';
    const lineWidth = config.strokeWidth || 1.5;

    let svgContent = '';
    const midX = segmentWidth / 2;
    const midY = segmentHeight / 2;

    switch (segmentType) {
        case 'vertical':
            svgContent = `<line x1="${midX}" y1="0" x2="${midX}" y2="${segmentHeight}" stroke="${lineColor}" stroke-width="${lineWidth}"/>`;
            break;
        case 'l-connector':
            svgContent = `<path d="M${midX} 0 V${midY} H${segmentWidth}" stroke="${lineColor}" stroke-width="${lineWidth}" fill="none"/>`;
            break;
        case 't-connector':
            svgContent = `<path d="M${midX} 0 V${segmentHeight} M${midX} ${midY} H${segmentWidth}" stroke="${lineColor}" stroke-width="${lineWidth}" fill="none"/>`;
            break;
        case 'empty-guide':
        case 'empty-connector':
            break;
    }
    return `<svg width="${segmentWidth}" height="${segmentHeight}" xmlns="http://www.w3.org/2000/svg" class="tree-line-svg" style="vertical-align: top;">${svgContent}</svg>`;
}

export function renderTreeView(treeData, containerElement, aiTreeConfig) {
    console.log("[RenderTreeView START] aiTreeConfig:", aiTreeConfig ? JSON.parse(JSON.stringify(aiTreeConfig)) : "No AiTreeConfig");
    containerElement.innerHTML = '';             

    const searchInput = document.createElement('input');
    searchInput.type = 'text';
    searchInput.placeholder = 'Search tree...';
    searchInput.style.marginBottom = '10px';
    searchInput.style.padding = '8px';
    searchInput.style.width = 'calc(100% - 16px)';                 
    searchInput.style.boxSizing = 'border-box';
    searchInput.id = "treeViewSearchInput";
    containerElement.appendChild(searchInput);

    if (!treeData || treeData.length === 0) {
        const noDataMessage = document.createElement('p');
        noDataMessage.textContent = 'No hierarchical data to display.';
        containerElement.appendChild(noDataMessage);
        return;
    }

    const table = document.createElement('table');
    table.className = 'results-table tree-view-table';
    const tbody = table.createTBody();
    containerElement.appendChild(table);

    const thead = table.createTHead();
    const headerRow = thead.insertRow();
    const thName = document.createElement('th');
    thName.textContent = "Name / Group";
    thName.style.minWidth = '300px';
    headerRow.appendChild(thName);

    const uniqueAttributeKeys = new Set();
    const knownTreeNodeSchemaProperties = ['name', 'expandable', 'children', 'expandedByDefault'];

    function collectKeysFromLeaves(nodes) {
        if (!nodes) return;
        nodes.forEach(node => {
            if (!node.expandable || !(node.children && node.children.length > 0)) {
                for (const key in node) {
                    if (Object.prototype.hasOwnProperty.call(node, key)) {
                        if (!knownTreeNodeSchemaProperties.includes(key)) {
                            uniqueAttributeKeys.add(key);
                        }
                    }
                }
            }
            if (node.children && node.children.length > 0) {
                collectKeysFromLeaves(node.children);
            }
        });
    }

    collectKeysFromLeaves(treeData);
    console.log("[RenderTreeView] Keys collected for attributes:", Array.from(uniqueAttributeKeys));

    let displayAttributeKeys = [];
    const configuredAttributeOrder = [];

    if (aiTreeConfig) {
        (aiTreeConfig.coreLeafAttributeColumns || []).forEach(c => {
            const sk = SanitizeKeyForJsonJS(c);
            if (uniqueAttributeKeys.has(sk) && !configuredAttributeOrder.includes(sk)) configuredAttributeOrder.push(sk);
        });
        (aiTreeConfig.paths || []).forEach(p => {
            (p.contextualLeafColumns || []).forEach(c => {
                const sk = SanitizeKeyForJsonJS(c);
                if (uniqueAttributeKeys.has(sk) && !configuredAttributeOrder.includes(sk)) configuredAttributeOrder.push(sk);
            });
        });
    }

    uniqueAttributeKeys.forEach(key => {
        if (!configuredAttributeOrder.includes(key)) {
            if (!(aiTreeConfig && aiTreeConfig.leafIdentifierColumn && key.toLowerCase() === aiTreeConfig.leafIdentifierColumn.toLowerCase())) {
                configuredAttributeOrder.push(key);
            }
        }
    });
    displayAttributeKeys = configuredAttributeOrder;
    console.log("[RenderTreeView] Final displayAttributeKeys for cells (ordered):", displayAttributeKeys);

    displayAttributeKeys.forEach(attrKey => {
        const thAttr = document.createElement('th');
        thAttr.textContent = attrKey;
        headerRow.appendChild(thAttr);
    });

    const initialTreeDataLength = treeData.length;
    treeData.forEach((rootNode, index) => {
        const isInitiallyExpanded = rootNode.expandedByDefault === true || false;
        renderTreeNodeRecursive(
            rootNode,
            tbody,
            0,
            displayAttributeKeys,
            aiTreeConfig,
            false,
            [],
            index === initialTreeDataLength - 1,
            initialTreeDataLength,
            isInitiallyExpanded
        );
    });

    searchInput.addEventListener('input', function () {
        applySearchFilter(tbody, this.value.trim().toLowerCase());
    });

    console.log("[RenderTreeView END]");
}

function renderTreeNodeRecursive(
    node,
    parentElement,
    depth,
    attributeKeysToRender,
    aiTreeConfig,
    isRowInitiallyHidden,
    parentPrefixSegments,
    isLastAmongSiblings,
    siblingCount,
    isInitiallyExpanded
) {
    if (!Array.isArray(parentPrefixSegments)) {
        console.error(`[RenderTreeNodeRecursive] Corrected: parentPrefixSegments was not an array! Received:`, parentPrefixSegments, `at Depth: ${depth}, Node Name: ${node.name}. Defaulting to [].`);
        parentPrefixSegments = [];
    }

    const row = parentElement.insertRow();
    row.classList.add(`tree-node-level-${depth}`);
    row.dataset.depth = depth;
    if (node.name) row.dataset.nodeName = node.name;

    if (isRowInitiallyHidden) {
        row.style.display = 'none';
    }
    const nameCell = row.insertCell();
    nameCell.classList.add('tree-name-cell');

    const prefixLineContainer = document.createElement('div');
    prefixLineContainer.className = 'tree-prefix-lines';
    prefixLineContainer.style.display = 'inline-flex';

    const svgConfig = {
        width: 18,
        height: 22,
        color: '#4a4a4a',
        strokeWidth: 1.5
    };

    parentPrefixSegments.forEach(segmentType => {
        const guideSvgContainer = document.createElement('span');
        guideSvgContainer.className = `tree-line-guide tree-line-${segmentType}`;
        guideSvgContainer.innerHTML = getTreeLineSVG(segmentType, svgConfig);
        prefixLineContainer.appendChild(guideSvgContainer);
    });

    let connectorType = '';
    if (depth === 0 && siblingCount === 1 && !(node.children && node.children.length > 0)) {
        connectorType = 'empty-connector';
    } else if (isLastAmongSiblings) {
        connectorType = 'l-connector';
    } else {
        connectorType = 't-connector';
    }

    const connectorSvgContainer = document.createElement('span');
    connectorSvgContainer.className = `tree-line-connector tree-line-${connectorType}`;
    connectorSvgContainer.innerHTML = getTreeLineSVG(connectorType, svgConfig);
    prefixLineContainer.appendChild(connectorSvgContainer);
    nameCell.appendChild(prefixLineContainer);

    const hasChildren = node.children && node.children.length > 0;

    if (node.expandable && hasChildren) {
        row.classList.add('expandable-node');
        const toggleButton = document.createElement('button');
        toggleButton.className = 'tree-toggle-btn';
        toggleButton.setAttribute('aria-expanded', String(isInitiallyExpanded));
        toggleButton.innerHTML = isInitiallyExpanded ? '&#x25BC;' : '&#x25B6;';                     

        toggleButton.onclick = (e) => {
            e.stopPropagation();
            const isCurrentlyExpanded = toggleButton.getAttribute('aria-expanded') === 'true';
            const newExpandedState = !isCurrentlyExpanded;
            toggleButton.setAttribute('aria-expanded', String(newExpandedState));
            toggleButton.innerHTML = newExpandedState ? '&#x25BC;' : '&#x25B6;';
            let currentRowToggler = row.nextElementSibling;
            while (currentRowToggler) {
                const currentDepth = parseInt(currentRowToggler.dataset.depth, 10);
                if (isNaN(currentDepth) || currentDepth <= depth) break;

                if (currentDepth === depth + 1) {         
                    if (document.querySelector('input[placeholder="Search tree..."]').value.trim() === '') {
                        currentRowToggler.style.display = newExpandedState ? '' : 'none';
                    } else {
                        if (!newExpandedState) {
                            currentRowToggler.style.display = 'none';
                        } else {
                            const searchTerm = document.querySelector('input[placeholder="Search tree..."]').value.trim().toLowerCase();
                            if (searchTerm) {                     
                                let childTextContent = '';
                                for (let i = 0; i < currentRowToggler.cells.length; i++) {
                                    childTextContent += currentRowToggler.cells[i].textContent.toLowerCase() + ' ';
                                }
                                if (!childTextContent.includes(searchTerm) && !isAncestorOfMatch(currentRowToggler, searchTerm, parentElement.querySelectorAll('tr'))) {
                                    currentRowToggler.style.display = 'none';
                                } else {
                                    currentRowToggler.style.display = '';                                 
                                }
                            } else {
                                currentRowToggler.style.display = '';                         
                            }
                        }
                    }
                } else if (currentDepth > depth + 1 && !newExpandedState) {
                    currentRowToggler.style.display = 'none';
                    const deeperChildToggle = currentRowToggler.querySelector('.tree-toggle-btn[aria-expanded="true"]');
                    if (deeperChildToggle) {
                        deeperChildToggle.setAttribute('aria-expanded', 'false');
                        deeperChildToggle.innerHTML = '&#x25B6;';
                    }
                }
                currentRowToggler = currentRowToggler.nextElementSibling;
            }
        };
        nameCell.appendChild(toggleButton);
    } else {
        row.classList.add(hasChildren ? 'non-expandable-parent' : 'leaf-node');
        const spacer = document.createElement('span');
        spacer.className = 'tree-node-button-spacer';
        nameCell.appendChild(spacer);
    }

    nameCell.appendChild(document.createTextNode(` ${node.name || 'N/A'}`));
    if (!node.expandable || !hasChildren) {                                     
        attributeKeysToRender.forEach(keyFromHeader => {
            const attrCell = row.insertCell();
            const value = Object.prototype.hasOwnProperty.call(node, keyFromHeader) ? node[keyFromHeader] : undefined;
            if (typeof value === 'number' && !isNaN(value)) {
                attrCell.textContent = value.toLocaleString();
                attrCell.classList.add('numeric-cell');
            } else {
                attrCell.textContent = (value !== undefined && value !== null) ? String(value) : '';
            }
        });
    } else {             
        attributeKeysToRender.forEach(() => {
            const emptyAttrCell = row.insertCell();
            emptyAttrCell.innerHTML = '&nbsp;';
        });
    }

    if (hasChildren) {
        const childrenShouldBeHidden = !isInitiallyExpanded || isRowInitiallyHidden;
        let nextParentPrefixSegmentsForChild;
        if (depth === 0 && siblingCount === 1) {
            nextParentPrefixSegmentsForChild = [];
        } else {
            nextParentPrefixSegmentsForChild = [...parentPrefixSegments];
            nextParentPrefixSegmentsForChild.push(isLastAmongSiblings ? 'empty-guide' : 'vertical');
        }

        node.children.forEach((childNode, index) => {
            const isLastNestedChild = index === node.children.length - 1;
            const childIsInitiallyExpanded = (childNode.expandedByDefault === true) && isInitiallyExpanded;

            renderTreeNodeRecursive(
                childNode,
                parentElement,
                depth + 1,
                attributeKeysToRender,
                aiTreeConfig,
                childrenShouldBeHidden,
                nextParentPrefixSegmentsForChild,
                isLastNestedChild,
                node.children.length,
                childIsInitiallyExpanded
            );
        });
    }
}

function isAncestorOfMatch(row, searchTerm, allRowsArray) {
    const depth = parseInt(row.dataset.depth, 10);
    let isAncestor = false;
    let pastCurrentRow = false;
    for (const otherRow of allRowsArray) {
        if (otherRow === row) {
            pastCurrentRow = true;
            continue;
        }
        if (!pastCurrentRow) continue;

        const otherDepth = parseInt(otherRow.dataset.depth, 10);
        if (otherDepth <= depth) break;                 

        let otherRowTextContent = '';
        for (let i = 0; i < otherRow.cells.length; i++) {
            otherRowTextContent += otherRow.cells[i].textContent.toLowerCase() + ' ';
        }
        if (otherRowTextContent.includes(searchTerm)) {
            isAncestor = true;
            break;
        }
    }
    return isAncestor;
}
 
function applySearchFilter(tbody, searchTerm) {
    const allRows = Array.from(tbody.querySelectorAll('tr'));

    if (!searchTerm) {
                                
        allRows.forEach(row => {
            const depth = parseInt(row.dataset.depth, 10);
            if (depth === 0) {
                row.style.display = '';                     
                                                                
            } else {
                                                        
                let parentRow = null;
                let currentIndex = allRows.indexOf(row);
                for (let i = currentIndex - 1; i >= 0; i--) {
                    if (parseInt(allRows[i].dataset.depth, 10) === depth - 1) {
                        parentRow = allRows[i];
                        break;
                    }
                }

                if (parentRow) {
                    const parentToggle = parentRow.querySelector('.tree-toggle-btn');
                    if (parentRow.style.display === 'none' || (parentToggle && parentToggle.getAttribute('aria-expanded') === 'false')) {
                        row.style.display = 'none';
                    } else {
                        row.style.display = '';
                    }
                } else {
                                                                    
                    row.style.display = 'none';
                }
            }
        });
        return;
    }
    const rowsToShow = new Set();
    allRows.forEach(row => {
        let rowTextContent = '';
                                                
        for (let i = 0; i < row.cells.length; i++) {
            rowTextContent += row.cells[i].textContent.toLowerCase() + ' ';
        }

        if (rowTextContent.includes(searchTerm)) {
            rowsToShow.add(row);
            let currentDepth = parseInt(row.dataset.depth, 10);
            let currentIndex = allRows.indexOf(row);
            for (let i = currentIndex - 1; i >= 0; i--) {
                const potentialAncestorRow = allRows[i];
                const ancestorDepth = parseInt(potentialAncestorRow.dataset.depth, 10);
                if (ancestorDepth < currentDepth) {                             
                    rowsToShow.add(potentialAncestorRow);
                    currentDepth = ancestorDepth;                                         
                }
                if (currentDepth === 0) break;                         
            }
        }
    });

                    
    allRows.forEach(row => {
        if (rowsToShow.has(row)) {
            row.style.display = '';                                           
                                            
        } else {
            row.style.display = 'none';
        }
    });
}