import { tooltip } from './domElements.js';                 

const d3 = window.d3;

const DEFAULT_CHART_WIDTH_FALLBACK = 800;                                         
const MIN_PLOT_AREA_WIDTH = 800;                                          
const LEGEND_WIDTH = 250;                                             

const FIXED_PIE_DONUT_WIDTH = 450;                                
const FIXED_PIE_DONUT_HEIGHT = 450;                               
const PIE_DONUT_MARGIN = 40;                                              

const DEFAULT_MARGIN_TOP = 20;
const DEFAULT_MARGIN_LEFT = 60;
const DEFAULT_MARGIN_BOTTOM = 60;                                         
function ensureTooltip() {
    if (!tooltip) {
        console.error("D3 tooltip is not initialized or available.");
        return {
            style: () => ({
                html: () => ({
                    style: () => ({})
                })
            })
        };
    }
    return tooltip;
}

export function renderBarChart(data, container) {
    if (!d3) { console.error("D3 is not available."); return; }
    const localTooltip = ensureTooltip();
    container.innerHTML = '';
    const isMultiSeries = data.length > 0 && data[0].hasOwnProperty('series');

    const margin = {
        top: DEFAULT_MARGIN_TOP,
        right: LEGEND_WIDTH,
        bottom: DEFAULT_MARGIN_BOTTOM,
        left: DEFAULT_MARGIN_LEFT
    };
    const availableWidth = container.clientWidth || DEFAULT_CHART_WIDTH_FALLBACK;
    const plotWidth = Math.max(MIN_PLOT_AREA_WIDTH, availableWidth - margin.left - margin.right);
    const plotHeight = 400 - margin.top - margin.bottom;                                             

    const svg = d3.select(container)
        .append("svg")
        .attr("width", plotWidth + margin.left + margin.right)
        .attr("height", plotHeight + margin.top + margin.bottom)
        .append("g")
        .attr("transform", `translate(${margin.left},${margin.top})`);

    if (isMultiSeries) {
        const seriesNames = data.map(d => d.series);
        const groupLabels = [...new Set(data.flatMap(d => d.values.map(v => v.label)))];

        const x0 = d3.scaleBand()
            .domain(groupLabels)
            .rangeRound([0, plotWidth])
            .paddingInner(0.1);

        const x1 = d3.scaleBand()
            .domain(seriesNames)
            .rangeRound([0, x0.bandwidth()])
            .padding(0.05);

        const allValues = data.flatMap(d => d.values.map(v => v.value));
        const y = d3.scaleLinear()
            .domain([0, d3.max(allValues) > 0 ? d3.max(allValues) : 1]).nice()
            .rangeRound([plotHeight, 0]);

        const color = d3.scaleOrdinal(d3.schemeTableau10).domain(seriesNames);

        svg.append("g")
            .selectAll("g")
            .data(data)
            .join("g")
            .attr("fill", d => color(d.series))
            .selectAll("rect")
            .data(d => d.values.map(v => ({ ...v, series: d.series })))
            .join("rect")
            .attr("x", d => x0(d.label) + x1(d.series))
            .attr("y", d => y(d.value))
            .attr("width", x1.bandwidth())
            .attr("height", d => Math.max(0, plotHeight - y(d.value)))
            .on("mouseover", () => localTooltip.style("opacity", 1))
            .on("mousemove", (event, d) => localTooltip.html(`<strong>${d.series}</strong><br/>${d.label}: ${d.value.toLocaleString()}`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px"))
            .on("mouseout", () => localTooltip.style("opacity", 0));

        svg.append("g")
            .attr("transform", `translate(0,${plotHeight})`)
            .call(d3.axisBottom(x0))
            .selectAll("text")
            .attr("transform", "rotate(-45)")
            .style("text-anchor", "end")
            .style("fill", "#333");

        svg.append("g")
            .call(d3.axisLeft(y))
            .selectAll("text")
            .style("fill", "#333");

        const legend = svg.selectAll(".legend")
            .data(seriesNames)
            .enter().append("g")
            .attr("class", "legend")
            .attr("transform", (d, i) => `translate(0,${i * 20})`);

        legend.append("rect")
            .attr("x", plotWidth + 20)                         
            .attr("width", 18)
            .attr("height", 18)
            .style("fill", color);

        legend.append("text")
            .attr("x", plotWidth + 45)                         
            .attr("y", 9)
            .attr("dy", ".35em")
            .style("text-anchor", "start")
            .text(d => d);

    } else {                 
        const x = d3.scaleBand().domain(data.map(d => d.label)).range([0, plotWidth]).padding(0.2);
        const y = d3.scaleLinear().domain([0, d3.max(data, d => d.value) > 0 ? d3.max(data, d => d.value) : 1]).nice().range([plotHeight, 0]);

        svg.selectAll(".bar").data(data).enter().append("rect").attr("class", "bar").attr("x", d => x(d.label)).attr("y", d => y(d.value)).attr("width", x.bandwidth()).attr("height", d => Math.max(0, plotHeight - y(d.value))).attr("fill", "#007bff")
            .on("mouseover", () => localTooltip.style("opacity", 1))
            .on("mousemove", (event, d) => localTooltip.html(`<strong>${d.label}</strong><br/>Value: ${d.value.toLocaleString()}`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px"))
            .on("mouseout", () => localTooltip.style("opacity", 0));

        svg.append("g").attr("transform", `translate(0,${plotHeight})`).call(d3.axisBottom(x)).selectAll("text").attr("transform", "rotate(-45)").style("text-anchor", "end").style("fill", "#333");
        svg.append("g").call(d3.axisLeft(y)).selectAll("text").style("fill", "#333");
    }
}

export function renderLineChart(data, container) {
    if (!d3) { console.error("D3 is not available."); return; }
    const localTooltip = ensureTooltip();
    container.innerHTML = '';
    const isMultiSeries = data.length > 0 && data[0].hasOwnProperty('series');

    const margin = {
        top: DEFAULT_MARGIN_TOP,
        right: LEGEND_WIDTH,
        bottom: DEFAULT_MARGIN_BOTTOM,
        left: DEFAULT_MARGIN_LEFT
    };
    const availableWidth = container.clientWidth || DEFAULT_CHART_WIDTH_FALLBACK;
    const plotWidth = Math.max(MIN_PLOT_AREA_WIDTH, availableWidth - margin.left - margin.right);
    const plotHeight = 400 - margin.top - margin.bottom;                             

    const svg = d3.select(container)
        .append("svg")
        .attr("width", plotWidth + margin.left + margin.right)
        .attr("height", plotHeight + margin.top + margin.bottom)
        .append("g")
        .attr("transform", `translate(${margin.left},${margin.top})`);

    const allData = isMultiSeries ? data.flatMap(d => d.values) : data;
    if (allData.length === 0) return;

    const x = d3.scalePoint()
        .domain([...new Set(allData.map(d => d.label))].sort())
        .range([0, plotWidth]);

    const y = d3.scaleLinear()
        .domain([0, d3.max(allData, d => d.value) > 0 ? d3.max(allData, d => d.value) : 1]).nice()
        .range([plotHeight, 0]);

    svg.append("g")
        .attr("transform", `translate(0,${plotHeight})`)
        .call(d3.axisBottom(x))
        .selectAll("text")
        .attr("transform", "rotate(-45)")
        .style("text-anchor", "end")
        .style("fill", "#333");

    svg.append("g")
        .call(d3.axisLeft(y))
        .selectAll("text")
        .style("fill", "#333");

    if (isMultiSeries) {
        const seriesNames = data.map(d => d.series);
        const color = d3.scaleOrdinal(d3.schemeTableau10).domain(seriesNames);

        svg.selectAll(".line")
            .data(data)
            .join("path")
            .attr("fill", "none")
            .attr("stroke", d => color(d.series))
            .attr("stroke-width", 2.5)
            .attr("d", d => d3.line()
                .x(item => x(item.label))
                .y(item => y(item.value))
                (d.values.sort((a, b) => x(a.label) - x(b.label)))
            );

        const points = svg.selectAll("g.series-points")
            .data(data)
            .enter().append("g")
            .attr("class", "series-points")
            .style("fill", d => color(d.series));

        points.selectAll("circle")
            .data(d => d.values.map(v => ({ ...v, series: d.series })))
            .enter().append("circle")
            .attr("cx", d => x(d.label))
            .attr("cy", d => y(d.value))
            .attr("r", 5)
            .on("mouseover", () => localTooltip.style("opacity", 1))
            .on("mousemove", (event, d) => localTooltip.html(`<strong>${d.series}</strong><br/>${d.label}: ${d.value.toLocaleString()}`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px"))
            .on("mouseout", () => localTooltip.style("opacity", 0));

        const legend = svg.selectAll(".legend")
            .data(seriesNames)
            .enter().append("g")
            .attr("class", "legend")
            .attr("transform", (d, i) => `translate(0,${i * 20})`);

        legend.append("rect")
            .attr("x", plotWidth + 20)
            .attr("width", 18)
            .attr("height", 18)
            .style("fill", color);

        legend.append("text")
            .attr("x", plotWidth + 45)
            .attr("y", 9)
            .attr("dy", ".35em")
            .style("text-anchor", "start")
            .text(d => d);
    } else {                 
        svg.append("path")
            .datum(allData.sort((a, b) => x(a.label) - x(b.label)))
            .attr("fill", "none")
            .attr("stroke", "#007bff")
            .attr("stroke-width", 2)
            .attr("d", d3.line().x(d => x(d.label)).y(d => y(d.value)));

        svg.selectAll(".dot")
            .data(allData)
            .enter().append("circle")
            .attr("class", "dot")
            .attr("cx", d => x(d.label))
            .attr("cy", d => y(d.value))
            .attr("r", 5)
            .attr("fill", "#007bff")
            .on("mouseover", () => localTooltip.style("opacity", 1))
            .on("mousemove", (event, d) => localTooltip.html(`<strong>${d.label}</strong><br/>Value: ${d.value.toLocaleString()}`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px"))
            .on("mouseout", () => localTooltip.style("opacity", 0));
    }
}

export function renderPieChart(data, container) {
    if (!d3) { console.error("D3 is not available."); return; }
    const localTooltip = ensureTooltip();
    container.innerHTML = '';
    const isMultiSeries = data.length > 0 && data[0].hasOwnProperty('series');

    let chartData = data;
    if (isMultiSeries) {
        chartData = data.flatMap(series =>
            series.values.map(item => ({
                label: `${series.series} - ${item.label}`,
                value: item.value
            }))
        );
    }
    if (chartData.length === 0) return;

    const radius = Math.min(FIXED_PIE_DONUT_WIDTH, FIXED_PIE_DONUT_HEIGHT) / 2 - PIE_DONUT_MARGIN;

    const svg = d3.select(container).append("svg")
        .attr("width", FIXED_PIE_DONUT_WIDTH)
        .attr("height", FIXED_PIE_DONUT_HEIGHT)
        .append("g")
        .attr("transform", `translate(${FIXED_PIE_DONUT_WIDTH / 2},${FIXED_PIE_DONUT_HEIGHT / 2})`);

    const color = d3.scaleOrdinal().domain(chartData.map(d => d.label)).range(d3.schemeTableau10);
    const pie = d3.pie().value(d => d.value).sort(null);
    const arc = d3.arc().innerRadius(0).outerRadius(radius);

    const g = svg.selectAll('path').data(pie(chartData)).enter().append('g');

    g.append('path').attr('d', arc).attr('fill', d => color(d.data.label)).attr("stroke", "white").style("stroke-width", "2px")
        .on("mouseover", (event, d_arc) => {                                         
            d3.select(event.currentTarget).transition().duration(200).attr('d', d3.arc().innerRadius(0).outerRadius(radius * 1.05));
            localTooltip.style("opacity", 1);
        })
        .on("mousemove", (event, d_arc) => {
            const total = d3.sum(chartData, item => item.value);
            const percent = total > 0 ? (d_arc.data.value / total * 100).toFixed(2) : 0;
            localTooltip.html(`<strong>${d_arc.data.label}</strong><br/>Value: ${d_arc.data.value.toLocaleString()}<br/>(${percent}%)`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px");
        })
        .on("mouseout", (event, d_arc) => {
            d3.select(event.currentTarget).transition().duration(200).attr('d', arc);
            localTooltip.style("opacity", 0);
        });

    g.append("text")
        .attr("transform", d_arc => `translate(${arc.centroid(d_arc)})`)
        .attr("dy", ".35em")
        .style("text-anchor", "middle")
        .style("fill", "#fff")
        .style("font-size", "10px")
        .text(d_arc => {
            const total = d3.sum(chartData, item => item.value);
            const percent = total > 0 ? (d_arc.data.value / total * 100) : 0;
            return percent > 5 ? d_arc.data.label : '';
        });
}

export function renderAreaChart(data, container) {
    if (!d3) { console.error("D3 is not available."); return; }
    const localTooltip = ensureTooltip();
    container.innerHTML = '';
    const isMultiSeries = data.length > 0 && data[0].hasOwnProperty('series');

    const margin = {
        top: DEFAULT_MARGIN_TOP,
        right: LEGEND_WIDTH,
        bottom: DEFAULT_MARGIN_BOTTOM,
        left: DEFAULT_MARGIN_LEFT
    };
    const availableWidth = container.clientWidth || DEFAULT_CHART_WIDTH_FALLBACK;
    const plotWidth = Math.max(MIN_PLOT_AREA_WIDTH, availableWidth - margin.left - margin.right);
    const plotHeight = 400 - margin.top - margin.bottom;                     

    const svg = d3.select(container)
        .append("svg")
        .attr("width", plotWidth + margin.left + margin.right)
        .attr("height", plotHeight + margin.top + margin.bottom)
        .append("g")
        .attr("transform", `translate(${margin.left},${margin.top})`);

    const allDataBase = isMultiSeries ? data.flatMap(d => d.values) : data;
    if (allDataBase.length === 0) return;

    if (isMultiSeries) {
        const seriesNames = data.map(d => d.series);
        const groupLabels = [...new Set(data.flatMap(d => d.values.map(v => v.label)))].sort();

        const pivotedData = groupLabels.map(label => {
            const obj = { label };
            data.forEach(series => {
                const found = series.values.find(v => v.label === label);
                obj[series.series] = found ? found.value : 0;
            });
            return obj;
        });

        const color = d3.scaleOrdinal(d3.schemeTableau10).domain(seriesNames);
        const stack = d3.stack().keys(seriesNames);
        const stackedData = stack(pivotedData);

        const x = d3.scalePoint()
            .domain(groupLabels)
            .range([0, plotWidth]);

        const y = d3.scaleLinear()
            .domain([0, d3.max(stackedData, d_stack => d3.max(d_stack, item => item[1])) > 0 ? d3.max(stackedData, d_stack => d3.max(d_stack, item => item[1])) : 1]).nice()
            .range([plotHeight, 0]);

        const area = d3.area()
            .x(d_area => x(d_area.data.label))
            .y0(d_area => y(d_area[0]))
            .y1(d_area => y(d_area[1]));

        svg.selectAll("mylayers")
            .data(stackedData)
            .join("path")
            .style("fill", d_stack => color(d_stack.key))
            .attr("d", area)
            .on("mouseover", () => localTooltip.style("opacity", 1))
            .on("mousemove", function (event, d_stack) {
                localTooltip.html(`<strong>${d_stack.key}</strong>`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px");
            })
            .on("mouseout", () => localTooltip.style("opacity", 0));

        svg.append("g")
            .attr("transform", `translate(0,${plotHeight})`)
            .call(d3.axisBottom(x))
            .selectAll("text")
            .attr("transform", "rotate(-45)")
            .style("text-anchor", "end")
            .style("fill", "#333");


        svg.append("g").call(d3.axisLeft(y)).selectAll("text").style("fill", "#333");


        const legend = svg.selectAll(".legend")
            .data(seriesNames)
            .enter().append("g")
            .attr("class", "legend")
            .attr("transform", (d_legend, i) => `translate(0,${i * 20})`);

        legend.append("rect")
            .attr("x", plotWidth + 20)
            .attr("width", 18)
            .attr("height", 18)
            .style("fill", color);

        legend.append("text")
            .attr("x", plotWidth + 45)
            .attr("y", 9)
            .attr("dy", ".35em")
            .style("text-anchor", "start")
            .text(d_legend => d_legend);

    } else {                 
        const x = d3.scalePoint().domain(allDataBase.map(d => d.label).sort()).range([0, plotWidth]);
        const y = d3.scaleLinear().domain([0, d3.max(allDataBase, d => d.value) > 0 ? d3.max(allDataBase, d => d.value) : 1]).nice().range([plotHeight, 0]);

        svg.append("path")
            .datum(allDataBase.sort((a, b) => x(a.label) - x(b.label)))
            .attr("fill", "rgba(0, 123, 255, 0.3)")
            .attr("stroke", "#007bff")
            .attr("stroke-width", 2)
            .attr("d", d3.area().x(d_area => x(d_area.label)).y0(plotHeight).y1(d_area => y(d_area.value)));

        svg.append("g").attr("transform", `translate(0,${plotHeight})`).call(d3.axisBottom(x)).selectAll("text").attr("transform", "rotate(-45)").style("text-anchor", "end").style("fill", "#333");
        svg.append("g").call(d3.axisLeft(y)).selectAll("text").style("fill", "#333");
    }
}

export function renderScatterChart(data, container) {
    if (!d3) { console.error("D3 is not available."); return; }
    const localTooltip = ensureTooltip();
    container.innerHTML = '';
    const isMultiSeries = data.length > 0 && data[0].hasOwnProperty('series');

    const margin = {
        top: DEFAULT_MARGIN_TOP,
        right: LEGEND_WIDTH,
        bottom: DEFAULT_MARGIN_BOTTOM,
        left: DEFAULT_MARGIN_LEFT
    };
    const availableWidth = container.clientWidth || DEFAULT_CHART_WIDTH_FALLBACK;
    const plotWidth = Math.max(MIN_PLOT_AREA_WIDTH, availableWidth - margin.left - margin.right);
    const plotHeight = 400 - margin.top - margin.bottom;                     

    const svg = d3.select(container).append("svg")
        .attr("width", plotWidth + margin.left + margin.right)
        .attr("height", plotHeight + margin.top + margin.bottom)
        .append("g")
        .attr("transform", `translate(${margin.left},${margin.top})`);

    const getX = d => d.x ?? d.label;
    const getY = d => d.y ?? d.value;

    const allDataBase = isMultiSeries ? data.flatMap(d => d.values.map(v => ({ ...v, series: d.series }))) : data;
    if (allDataBase.length === 0) return;


    if (isMultiSeries) {
        const seriesNames = data.map(d => d.series);
        const color = d3.scaleOrdinal(d3.schemeTableau10).domain(seriesNames);

        const x = d3.scaleLinear()
            .domain(d3.extent(allDataBase, getX)).nice()
            .range([0, plotWidth]);

        const y = d3.scaleLinear()
            .domain([0, d3.max(allDataBase, getY) > 0 ? d3.max(allDataBase, getY) : 1]).nice()
            .range([plotHeight, 0]);

        svg.selectAll(".dot")
            .data(allDataBase)
            .enter().append("circle")
            .attr("class", "dot")
            .attr("cx", d_dot => x(getX(d_dot)))
            .attr("cy", d_dot => y(getY(d_dot)))
            .attr("r", 5)
            .style("fill", d_dot => color(d_dot.series))
            .on("mouseover", () => localTooltip.style("opacity", 1))
            .on("mousemove", (event, d_dot) => localTooltip.html(`<strong>${d_dot.series} - ${d_dot.label || getX(d_dot)}</strong><br/>X: ${getX(d_dot)}, Y: ${getY(d_dot).toLocaleString()}`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px"))
            .on("mouseout", () => localTooltip.style("opacity", 0));

        svg.append("g").attr("transform", `translate(0,${plotHeight})`).call(d3.axisBottom(x)).selectAll("text").style("fill", "#333");
        svg.append("g").call(d3.axisLeft(y)).selectAll("text").style("fill", "#333");


        const legend = svg.selectAll(".legend")
            .data(seriesNames)
            .enter().append("g")
            .attr("class", "legend")
            .attr("transform", (d_legend, i) => `translate(0,${i * 20})`);

        legend.append("rect")
            .attr("x", plotWidth + 20)
            .attr("width", 18)
            .attr("height", 18)
            .style("fill", color);

        legend.append("text")
            .attr("x", plotWidth + 45)
            .attr("y", 9)
            .attr("dy", ".35em")
            .style("text-anchor", "start")
            .text(d_legend => d_legend);

    } else {                 
        const x = d3.scaleLinear().domain(d3.extent(allDataBase, getX)).nice().range([0, plotWidth]);
        const y = d3.scaleLinear().domain([0, d3.max(allDataBase, getY) > 0 ? d3.max(allDataBase, getY) : 1]).nice().range([plotHeight, 0]);

        svg.selectAll(".dot").data(allDataBase).enter().append("circle")
            .attr("class", "dot")
            .attr("cx", d_dot => x(getX(d_dot)))
            .attr("cy", d_dot => y(getY(d_dot)))
            .attr("r", 5)
            .style("fill", "#007bff")
            .on("mouseover", () => localTooltip.style("opacity", 1))
            .on("mousemove", (event, d_dot) => localTooltip.html(`<strong>${d_dot.label || getX(d_dot)}</strong><br/>X: ${getX(d_dot)}, Y: ${getY(d_dot).toLocaleString()}`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px"))
            .on("mouseout", () => localTooltip.style("opacity", 0));

        svg.append("g").attr("transform", `translate(0,${plotHeight})`).call(d3.axisBottom(x)).selectAll("text").style("fill", "#333");
        svg.append("g").call(d3.axisLeft(y)).selectAll("text").style("fill", "#333");
    }
}

export function renderDoughnutChart(data, container) {
    if (!d3) { console.error("D3 is not available."); return; }
    const localTooltip = ensureTooltip();
    container.innerHTML = '';
    const isMultiSeries = data.length > 0 && data[0].hasOwnProperty('series');

    let chartData = data;
    if (isMultiSeries) {
        chartData = data.flatMap(series =>
            series.values.map(item => ({
                label: `${series.series} - ${item.label}`,
                value: item.value
            }))
        );
    }
    if (chartData.length === 0) return;

    const radius = Math.min(FIXED_PIE_DONUT_WIDTH, FIXED_PIE_DONUT_HEIGHT) / 2 - PIE_DONUT_MARGIN;

    const svg = d3.select(container).append("svg")
        .attr("width", FIXED_PIE_DONUT_WIDTH)
        .attr("height", FIXED_PIE_DONUT_HEIGHT)
        .append("g")
        .attr("transform", `translate(${FIXED_PIE_DONUT_WIDTH / 2},${FIXED_PIE_DONUT_HEIGHT / 2})`);

    const color = d3.scaleOrdinal().domain(chartData.map(d => d.label)).range(d3.schemeTableau10);
    const pie = d3.pie().value(d => d.value).sort(null);
    const arc = d3.arc().innerRadius(radius * 0.5).outerRadius(radius);
    const hoverArc = d3.arc().innerRadius(radius * 0.55).outerRadius(radius * 1.05);


    const g = svg.selectAll('path').data(pie(chartData)).enter().append('g');

    g.append('path').attr('d', arc).attr('fill', d_arc => color(d_arc.data.label)).attr("stroke", "white").style("stroke-width", "2px")
        .on("mouseover", function (event, d_arc) {
            d3.select(this).transition().duration(200).attr('d', hoverArc);
            localTooltip.style("opacity", 1);
        })
        .on("mousemove", (event, d_arc) => {
            const total = d3.sum(chartData, item => item.value);
            const percent = total > 0 ? (d_arc.data.value / total * 100).toFixed(2) : 0;
            localTooltip.html(`<strong>${d_arc.data.label}</strong><br/>Value: ${d_arc.data.value.toLocaleString()}<br/>(${percent}%)`).style("left", (event.pageX + 15) + "px").style("top", (event.pageY - 28) + "px");
        })
        .on("mouseout", function (event, d_arc) {
            d3.select(this).transition().duration(200).attr('d', arc);
            localTooltip.style("opacity", 0);
        });

    g.append("text")
        .attr("transform", d_arc => `translate(${arc.centroid(d_arc)})`)
        .attr("dy", ".35em")
        .style("text-anchor", "middle")
        .style("fill", "#fff")
        .style("font-size", "10px")
        .text(d_arc => {
            const total = d3.sum(chartData, item => item.value);
            const percent = total > 0 ? (d_arc.data.value / total * 100) : 0;
            return percent > 5 ? d_arc.data.label : '';
        });
}