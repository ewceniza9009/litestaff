import { showToast } from './toastService.js';
import { pdfChartRenderContainer, pdfMermaidRenderContainer } from './domElements.js';
import { renderBarChart, renderLineChart, renderPieChart, renderAreaChart, renderScatterChart, renderDoughnutChart } from './chartRenderer.js';

export function getRandomPrompts(sourceArray, count) {
    const shuffled = [...sourceArray].sort(() => 0.5 - Math.random());
    return shuffled.slice(0, count);
}

export function formatTimestamp(isoString) {
    if (!isoString) return '';
    const date = new Date(isoString);
    const pad = (num) => String(num).padStart(2, '0');
    const month = pad(date.getMonth() + 1);
    const day = pad(date.getDate());
    const year = date.getFullYear();
    let hours = date.getHours();
    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12;
    const displayHours = pad(hours);
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());
    return `${month}/${day}/${year} ${displayHours}:${minutes}:${seconds} ${ampm}`;
}

export function copyToClipboard(text) {
    if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(text).then(() => {
            showToast('Content copied to clipboard!', 'success');
        }).catch(err => {
            console.error('Failed to copy to clipboard using Clipboard API:', err);
            fallbackCopyToClipboard(text);
        });
    } else {
        fallbackCopyToClipboard(text);
    }
}

function fallbackCopyToClipboard(text) {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-9999px';
    textArea.style.top = '-9999px';
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    try {
        const successful = document.execCommand('copy');
        if (successful) {
            showToast('Content copied to clipboard!', 'success');
        } else {
            showToast('Failed to copy content.', 'error');
            console.error('Fallback copy: execCommand failed');
        }
    } catch (err) {
        showToast('Failed to copy content.', 'error');
        console.error('Fallback copy failed:', err);
    }
    document.body.removeChild(textArea);
}

export function exportDataToCSV(data, columnNames, filename) {
    let csv = [columnNames.join(',')];
    data.forEach(row => {
        const rowData = columnNames.map(colName => {
            let value = row[colName] ?? '';
            let stringValue = String(value);                 
            if (stringValue.includes('"')) {
                stringValue = stringValue.replace(/"/g, '""');
            }
            if (stringValue.includes(',')) {
                stringValue = `"${stringValue}"`;
            }
            return stringValue;
        });
        csv.push(rowData.join(','));
    });
    const csvFile = new Blob([csv.join('\n')], {
        type: 'text/csv;charset=utf-8;'
    });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(csvFile);
    link.setAttribute('download', filename);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function createTableForPdf(data) {
    const table = document.createElement('table');
    const thead = table.createTHead();
    const tbody = table.createTBody();
    const headerRow = thead.insertRow();

    if (!data || data.length === 0) {
        const th = document.createElement('th');
        th.textContent = "No data available for this table.";
        headerRow.appendChild(th);
        return table;                                 
    }

    const columnNames = Object.keys(data[0]);

    columnNames.forEach(key => {
        const th = document.createElement('th');
        th.textContent = key;
        headerRow.appendChild(th);
    });

    data.forEach(rowData => {
        const row = tbody.insertRow();
        columnNames.forEach(colName => {
            const cell = row.insertCell();
            let cellValue = rowData[colName];
            if (typeof cellValue === 'number' && !isNaN(cellValue)) {
                cell.textContent = cellValue.toLocaleString();             
            } else {
                cell.textContent = cellValue ?? 'NULL';
            }
        });
    });
    return table;
}

export async function exportChatToPDF(conversationHistory) {
    if (conversationHistory.length === 0 || (conversationHistory.length === 1 && !conversationHistory[0].content && !conversationHistory[0].tableData && !conversationHistory[0].chartData && !conversationHistory[0].mermaidDiagramSyntax)) {
        showToast("Nothing to export.", "info");
        return;
    }

    if (!window.jspdf || !window.jspdf.jsPDF) {
        showToast("jsPDF library not loaded.", "error");
        console.error("jsPDF is not loaded.");
        return;
    }
    const { jsPDF } = window.jspdf;
    const pdf = new jsPDF({ orientation: 'p', unit: 'pt', format: 'a4' });
    let y = 40;
    const pageHeight = pdf.internal.pageSize.getHeight();
    const pageWidth = pdf.internal.pageSize.getWidth();
    const margin = 40;
    const lineHeight = 12 * 1.2;

    pdf.setFontSize(18);
    pdf.text("Chat History", pageWidth / 2, y, { align: 'center' });
    y += lineHeight * 2;

    for (const message of conversationHistory) {
        if (y > pageHeight - margin * 2) {
            pdf.addPage();
            y = margin;
        }

        if (message.role === 'model' && !message.content && !message.tableData && !message.chartData && !message.mermaidDiagramSyntax && !message.insights) continue;
        if (message.role === 'user' && !message.content) continue;

        pdf.setFontSize(10);
        const role = message.role === 'user' ? 'You' : 'AI Assistant';
        pdf.setFont(undefined, 'bold');
        pdf.text(`${role}: (${formatTimestamp(message.timestamp)})`, margin, y);
        y += lineHeight;
        pdf.setFont(undefined, 'normal');

        if (message.content) {
            const cleanedContent = cleanTextForPdf(message.content);         
            const text = pdf.splitTextToSize(
                cleanedContent,
                pageWidth - margin * 2
            );
            if (y + (text.length * lineHeight) > pageHeight - margin) {
                pdf.addPage(); y = margin;
            }
            pdf.text(text, margin + 10, y);
            y += (text.length * lineHeight) + (lineHeight / 2);
        }

        if (message.insights) {
            if (y + lineHeight * 2 > pageHeight - margin) { pdf.addPage(); y = margin; }
            pdf.setFont(undefined, 'italic');
            pdf.text("Insights:", margin + 10, y);
            y += lineHeight;
            pdf.setFont(undefined, 'normal');                     

            const cleanedInsights = cleanTextForPdf(message.insights);         
            const insightsText = pdf.splitTextToSize(cleanedInsights, pageWidth - margin * 2 - 10);
            if (y + (insightsText.length * lineHeight) > pageHeight - margin) {
                pdf.addPage(); y = margin;
            }
            pdf.text(insightsText, margin + 15, y);
            y += (insightsText.length * lineHeight) + (lineHeight / 2);
        }


        if (message.role === 'model') {
            if (message.mermaidDiagramSyntax && window.mermaid && window.html2canvas) {
                if (y + 60 > pageHeight - margin) { pdf.addPage(); y = margin; }
                pdf.setFont(undefined, 'italic');
                pdf.text("Diagram:", margin + 10, y);
                y += lineHeight;
                pdf.setFont(undefined, 'normal');

                const mermaidHostId = `pdf-mermaid-host-${Date.now()}${Math.random().toString(16).slice(2)}`;
                const mermaidHostDiv = document.createElement('div');
                mermaidHostDiv.id = mermaidHostId;
                mermaidHostDiv.style.position = 'absolute';
                mermaidHostDiv.style.left = '-10000px';
                mermaidHostDiv.style.top = '-10000px';
                mermaidHostDiv.style.backgroundColor = 'white';
                mermaidHostDiv.style.padding = '1px';
                mermaidHostDiv.style.display = 'inline-block';
                document.body.appendChild(mermaidHostDiv);

                try {
                    const diagramSvgId = `pdf-mermaid-svg-${Date.now()}`;
                    const { svg } = await window.mermaid.render(diagramSvgId, message.mermaidDiagramSyntax);

                    if (svg && (!svg.includes("Syntax error in text") && !svg.includes("mermaidAPI.parseError"))) {
                        mermaidHostDiv.innerHTML = svg;
                        const svgElement = mermaidHostDiv.querySelector('svg');
                        if (!svgElement) throw new Error("SVG element not found after mermaid.render.");

                        await new Promise(r => setTimeout(r, 150));                     

                        const svgRect = svgElement.getBoundingClientRect();
                        if (!(svgRect.width > 0 && svgRect.height > 0)) {
                            console.warn("Mermaid PDF Export: SVG has zero/invalid dimensions before html2canvas.", svgRect);
                            mermaidHostDiv.style.width = Math.max(svgRect.width, 300) + 'px';
                            mermaidHostDiv.style.height = Math.max(svgRect.height, 200) + 'px';
                        }
                        const hostRect = mermaidHostDiv.getBoundingClientRect();                     
                        if (!(hostRect.width > 0 && hostRect.height > 0)) {
                            throw new Error(`mermaidHostDiv has zero dimensions (W: ${hostRect.width}, H: ${hostRect.height}) before html2canvas.`);
                        }

                        const canvas = await window.html2canvas(mermaidHostDiv, {
                            useCORS: true, backgroundColor: '#ffffff', scale: 1.5, logging: false,                             
                            width: hostRect.width, height: hostRect.height
                        });

                        const imgData = canvas.toDataURL('image/png');
                        if (!imgData || imgData === 'data:,' || imgData.length < 200) {
                            throw new Error(`html2canvas produced invalid image data (length: ${imgData.length}).`);
                        }

                        const imgProps = pdf.getImageProperties(imgData);
                        if (!imgProps || !(imgProps.width > 0 && imgProps.height > 0)) {
                            throw new Error("pdf.getImageProperties returned invalid dimensions for html2canvas output.");
                        }

                        const originalCanvasWidthPx = imgProps.width;
                        const originalCanvasHeightPx = imgProps.height;
                        const maxPdfWidthPt = pageWidth - margin * 2 - 10;
                        const maxPdfHeightPt = 480;                                         
                        let renderWidthPt = originalCanvasWidthPx * 0.75;
                        let renderHeightPt = originalCanvasHeightPx * 0.75;
                        const aspectRatio = originalCanvasWidthPx / originalCanvasHeightPx;

                        if (renderWidthPt > maxPdfWidthPt) {
                            renderWidthPt = maxPdfWidthPt;
                            renderHeightPt = renderWidthPt / aspectRatio;
                        }
                        if (renderHeightPt > maxPdfHeightPt) {
                            renderHeightPt = maxPdfHeightPt;
                            renderWidthPt = renderHeightPt * aspectRatio;
                        }
                        if (renderWidthPt > maxPdfWidthPt) {                     
                            renderWidthPt = maxPdfWidthPt;
                            renderHeightPt = renderWidthPt / aspectRatio;
                        }

                        if (!(renderWidthPt > 0 && renderHeightPt > 0)) {
                            throw new Error("Final scaled image dimensions are invalid.");
                        }

                        if (y + renderHeightPt > pageHeight - margin) { pdf.addPage(); y = margin; }
                        pdf.addImage(imgData, 'PNG', margin + 10, y, renderWidthPt, renderHeightPt);
                        y += renderHeightPt + lineHeight;

                    } else {
                        if (y + lineHeight > pageHeight - margin) { pdf.addPage(); y = margin; }
                        pdf.text("[Mermaid diagram had syntax errors or AI returned error]", margin + 15, y); y += lineHeight;
                    }
                } catch (e) {
                    console.error("Error during Mermaid PDF export (html2canvas path):", e);
                    if (y + lineHeight > pageHeight - margin) { pdf.addPage(); y = margin; }
                    const shortError = e.message ? e.message.substring(0, 70) : "Unknown error";
                    pdf.text(`[Err processing Mermaid: ${shortError}...]`, margin + 15, y); y += lineHeight;
                } finally {
                    if (mermaidHostDiv && mermaidHostDiv.parentNode === document.body) {
                        document.body.removeChild(mermaidHostDiv);
                    }
                }
            }

            const chartData = message.chartData?.length ? message.chartData : (message.chartData?.Value?.length ? message.chartData.Value : null);
            if (chartData && chartData.length > 0 && pdfChartRenderContainer && window.d3) {
                if (y + 60 > pageHeight - margin) { pdf.addPage(); y = margin; }
                pdf.setFont(undefined, 'italic');
                pdf.text("Chart:", margin + 10, y);
                y += lineHeight;
                pdf.setFont(undefined, 'normal');

                const chartType = message.chartType || 'bar';
                const renderFunctions = { 'bar': renderBarChart, 'line': renderLineChart, 'pie': renderPieChart, 'area': renderAreaChart, 'scatter': renderScatterChart, 'doughnut': renderDoughnutChart };
                const renderFunction = renderFunctions[chartType];

                if (renderFunction && window.canvg) {
                    pdfChartRenderContainer.innerHTML = '';
                    try {
                        renderFunction(chartData, pdfChartRenderContainer);
                        const svgElement = pdfChartRenderContainer.querySelector('svg');
                        if (svgElement) {
                            const canvas = document.createElement('canvas');
                            const svgClientRect = svgElement.getBoundingClientRect();
                            let svgWidth = svgClientRect.width > 0 ? svgClientRect.width : 600;
                            let svgHeight = svgClientRect.height > 0 ? svgClientRect.height : 400;
                            const scaleFactor = 1.5;

                            canvas.width = svgWidth * scaleFactor;
                            canvas.height = svgHeight * scaleFactor;
                            const ctx = canvas.getContext('2d');
                            ctx.scale(scaleFactor, scaleFactor);
                            const svgString = new XMLSerializer().serializeToString(svgElement);

                            await new Promise(resolveCanvg => {
                                window.canvg(canvas, svgString, {
                                    ignoreMouse: true, ignoreAnimation: true,
                                    scaleWidth: svgWidth, scaleHeight: svgHeight,
                                    renderCallback: function () {
                                        try {
                                            const imgData = canvas.toDataURL('image/png');
                                            const imgProps = pdf.getImageProperties(imgData);
                                            let imgWidthPt = imgProps.width * 0.75;
                                            let imgHeightPt = imgProps.height * 0.75;
                                            const maxImgWidthPt = pageWidth - margin * 2 - 10;
                                            const maxImgHeightPt = 200;                         

                                            if (imgWidthPt > maxImgWidthPt) { imgHeightPt = (maxImgWidthPt / imgWidthPt) * imgHeightPt; imgWidthPt = maxImgWidthPt; }
                                            if (imgHeightPt > maxImgHeightPt) { imgWidthPt = (maxImgHeightPt / imgHeightPt) * imgWidthPt; imgHeightPt = maxImgHeightPt; }
                                            if (imgWidthPt > maxImgWidthPt) { imgHeightPt = (maxImgWidthPt / imgWidthPt) * imgHeightPt; imgWidthPt = maxImgWidthPt; }             

                                            if (!(imgWidthPt > 0 && imgHeightPt > 0)) throw new Error("D3 Chart: Invalid scaled dimensions");

                                            if (y + imgHeightPt > pageHeight - margin) { pdf.addPage(); y = margin; }
                                            pdf.addImage(imgData, 'PNG', margin + 10, y, imgWidthPt, imgHeightPt);
                                            y += imgHeightPt + lineHeight;
                                        } catch (e) { console.error("Error adding D3 chart image to PDF:", e); }
                                        finally { resolveCanvg(); }
                                    }
                                });
                            });
                        }
                    } catch (e) { console.error("Error rendering D3 chart for PDF:", e); }
                    if (pdfChartRenderContainer) pdfChartRenderContainer.innerHTML = '';
                } else if (!window.canvg && renderFunction) {
                    if (y + lineHeight > pageHeight - margin) { pdf.addPage(); y = margin; }
                    pdf.text("[D3 chart cannot be rendered to PDF - canvg missing]", margin + 15, y); y += lineHeight;
                }
            }

            const tableData = message.tableData?.length ? message.tableData : (message.tableData?.Value?.length ? message.tableData.Value : null);
            if (tableData && tableData.length > 0) {
                if (typeof pdf.autoTable === 'function') {                                     
                    if (y + lineHeight * 3 > pageHeight - margin) { pdf.addPage(); y = margin; }
                    pdf.setFont(undefined, 'italic');
                    let tableTitle = "Table Data:";
                    if (message.isPivot) tableTitle = "Pivot Table Data:";
                    else if (message.isCrosstab) tableTitle = "Crosstab Data:";
                    pdf.text(tableTitle, margin + 10, y);
                    y += lineHeight;
                    pdf.setFont(undefined, 'normal');

                    let htmlTableElement;
                    try {
                        htmlTableElement = createTableForPdf(tableData);
                        if (!htmlTableElement || !htmlTableElement.rows || htmlTableElement.rows.length === 0 || (htmlTableElement.rows.length === 1 && htmlTableElement.rows[0].cells[0]?.textContent.includes("No data"))) {
                            throw new Error("createTableForPdf returned empty or 'no data' table.");
                        }
                    } catch (e) {
                        console.error("Error creating table for PDF:", e);
                        if (y + lineHeight > pageHeight - margin) { pdf.addPage(); y = margin; }
                        const shortError = e.message ? e.message.substring(0, 70) : "Unknown error";
                        pdf.text(`[Err preparing table: ${shortError}...]`, margin + 15, y); y += lineHeight;
                        htmlTableElement = null;
                    }

                    if (htmlTableElement) {
                        pdf.autoTable({
                            startY: y,
                            html: htmlTableElement,
                            theme: 'striped',
                            headStyles: { fillColor: [60, 142, 179] },
                            margin: { left: margin, right: margin },
                            tableWidth: 'auto',
                            styles: { cellPadding: 2, fontSize: 7, overflow: 'linebreak' },
                            didDrawPage: function (data) {
                                y = data.cursor.y + lineHeight / 2;
                            }
                        });
                        if (!pdf.autoTable.previous || y <= pdf.autoTable.previous.finalY) {
                            y = pdf.autoTable.previous ? pdf.autoTable.previous.finalY + lineHeight : y + lineHeight * 2;
                        }
                    }
                } else {                     
                    console.error("jsPDF-AutoTable plugin is not loaded or not correctly initialized. Tables will not be exported to PDF.");
                    if (y + lineHeight > pageHeight - margin) { pdf.addPage(); y = margin; }
                    pdf.setFont(undefined, 'normal');
                    pdf.setTextColor(255, 0, 0);                 
                    pdf.text("[Table export failed: jsPDF-AutoTable plugin not available]", margin + 15, y);
                    y += lineHeight;
                    pdf.setTextColor(0, 0, 0);             
                }
            }
        }
        y += lineHeight / 2;
    }
    pdf.save('chat-history.pdf');
    showToast("Chat history exported to PDF.", "success");
}

export async function exportMermaidDiagramAsImage(elementToCapture, filename = 'diagram.png') {
    if (!elementToCapture || typeof elementToCapture.getBoundingClientRect !== 'function') {
        console.error("Export Diagram: Invalid HTML element provided for capture.", elementToCapture);
        showToast("Invalid diagram element for export.", "error");
        return;
    }
    if (!window.html2canvas) {
        console.error("Export Diagram: html2canvas library is not loaded.");
        showToast("Export feature unavailable (html2canvas missing).", "error");
        return;
    }

    const originalBackgroundColor = elementToCapture.style.backgroundColor;
    if (!originalBackgroundColor || originalBackgroundColor === 'transparent') {
        elementToCapture.style.backgroundColor = '#FFFFFF';
    }

    try {
        await new Promise(resolve => setTimeout(resolve, 100));

        const canvas = await window.html2canvas(elementToCapture, {
            useCORS: true,
            backgroundColor: null,                                                     
            scale: 2,
            logging: false,
        });

        if (!originalBackgroundColor || originalBackgroundColor === 'transparent') {
            elementToCapture.style.backgroundColor = originalBackgroundColor;
        }


        const imgData = canvas.toDataURL('image/png');
        if (!imgData || imgData === 'data:,' || imgData.length < 200) {
            console.error("Export Diagram: html2canvas returned invalid image data.");
            throw new Error("Failed to generate image data from diagram.");
        }

        const link = document.createElement('a');
        link.href = imgData;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        showToast("Diagram exported as PNG.", "success");

    } catch (error) {
        console.error("Error exporting diagram as image:", error);
        showToast(`Failed to export diagram: ${error.message.substring(0, 100)}`, "error");
        if (!originalBackgroundColor || originalBackgroundColor === 'transparent') {
            elementToCapture.style.backgroundColor = originalBackgroundColor;
        }
    }
}

export async function exportD3ChartAsPNG(chartContainer, filename = 'd3_chart.png') {
    if (!chartContainer || typeof chartContainer.querySelector !== 'function') {
        showToast("Invalid chart container provided.", "error");
        console.error("Export D3 Chart: Invalid chartContainer element.");
        return;
    }
    if (!window.canvg) {                 
        showToast("Image export library (canvg) not loaded.", "error");
        console.error("Export D3 Chart: canvg library is not loaded.");
        return;
    }

    const svgElement = chartContainer.querySelector('svg');
    if (!svgElement) {
        showToast("Chart SVG element not found for export.", "error");
        console.error("Export D3 Chart: SVG element not found in chartContainer.");
        return;
    }

    const originalContainerBg = chartContainer.style.backgroundColor;
    chartContainer.style.backgroundColor = '#FFFFFF';                 

    try {
        await new Promise(resolve => setTimeout(resolve, 50));

        const canvas = document.createElement('canvas');

        const svgRect = svgElement.getBoundingClientRect();
        const svgWidth = svgRect.width > 0 ? svgRect.width : (parseInt(svgElement.getAttribute('width'), 10) || 800);
        const svgHeight = svgRect.height > 0 ? svgRect.height : (parseInt(svgElement.getAttribute('height'), 10) || 400);

        const scale = 2;                         
        canvas.width = svgWidth * scale;
        canvas.height = svgHeight * scale;

        const ctx = canvas.getContext('2d');
        ctx.fillStyle = '#FFFFFF';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.scale(scale, scale);

        const svgString = new XMLSerializer().serializeToString(svgElement);

        await new Promise((resolve, reject) => {
            window.canvg(canvas, svgString, {
                ignoreMouse: true,
                ignoreAnimation: true,
                scaleWidth: svgWidth,                             
                scaleHeight: svgHeight,                             
                renderCallback: function () {
                    try {
                        const imgData = canvas.toDataURL('image/png');
                        if (!imgData || imgData === 'data:,' || imgData.length < 200) {
                            console.error("Export D3 Chart: canvas.toDataURL returned invalid image data.");
                            reject(new Error("Failed to generate image data from chart."));
                            return;
                        }

                        const link = document.createElement('a');
                        link.href = imgData;
                        link.download = filename.endsWith('.png') ? filename : `${filename}.png`;             
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                        showToast("Chart exported as PNG.", "success");
                        resolve();
                    } catch (e) {
                        console.error("Error during D3 chart PNG conversion or download:", e);
                        showToast("Failed to export chart as PNG.", "error");
                        reject(e);
                    }
                }
            });
        });

    } catch (error) {
        console.error("Error exporting D3 chart as PNG:", error);
        showToast(`Failed to export chart: ${error.message.substring(0, 100)}`, "error");
    } finally {
        chartContainer.style.backgroundColor = originalContainerBg;             
    }
}

export function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');                         
}

function cleanTextForPdf(rawText) {
    if (typeof rawText !== 'string') {
        rawText = String(rawText || '');                                     
    }

    let text = rawText;

    text = text.replace(/<br\s*\/?>/gi, '\n');

    text = text.replace(/^###\s+/gm, '');                             
    text = text.replace(/^##\s+/gm, '');              
    text = text.replace(/^#\s+/gm, '');               

    text = text.replace(/^\*\s+/gm, '- ');                                         
    text = text.replace(/^-\s+/gm, '- ');                                         

    text = text.replace(/\*\*(.*?)\*\*/g, '$1');     
    text = text.replace(/__(.*?)__/g, '$1');        
    text = text.replace(/(?<!\w)\*(.*?)\*(?!\w)/g, '$1');
    text = text.replace(/`(.*?)`/g, '$1');          

    text = text.replace(/₱/g, 'PHP ');
    text = text.replace(/±/g, '+/-');
    text = text.replace(/&nbsp;/g, ' ');
    text = text.replace(/&amp;/g, '&');
    text = text.replace(/&lt;/g, '<');
    text = text.replace(/&gt;/g, '>');
    text = text.replace(/&quot;/g, '"');
    text = text.replace(/&apos;/g, "'");

    text = text.split('\n').map(line => line.replace(/\s+/g, ' ').trim()).join('\n');

    text = text.split('\n').filter(line => line.trim() !== '').join('\n');

    return text.trim();                         
}