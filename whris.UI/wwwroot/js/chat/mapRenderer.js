const L = window.L;

export function renderMap(mapData, parentContainer) {
    if (!L) {
        console.error("[LEAFLET DEBUG] Leaflet library (L) is NOT LOADED!");
        const errorDiv = document.createElement('div');
        errorDiv.className = 'leaflet-map-container message-section';                     
        errorDiv.style.border = "2px solid red";
        errorDiv.innerHTML = "<p style='color:red; padding:10px; text-align:center;'>Error: Mapping library (Leaflet) not loaded.</p>";
        parentContainer.appendChild(errorDiv);
        return;
    }

    if (mapData.mapLatitude && mapData.mapLongitude) {
        const mapId = `leaflet-map-${Date.now()}-${Math.random().toString(36).substr(2, 5)}`;
        const mapDiv = document.createElement('div');
        mapDiv.id = mapId;
        mapDiv.className = 'leaflet-map-container message-section';                 
        mapDiv.style.height = "300px";                         

        parentContainer.appendChild(mapDiv);

        requestAnimationFrame(() => {
            if (mapDiv.offsetHeight === 0 || mapDiv.offsetWidth === 0) {
                console.warn(`[LEAFLET DEBUG] Map container #${mapId} has zero height or width AFTER rAF. CSS for .leaflet-map-container height is crucial.`);
            }

            try {
                const lat = parseFloat(mapData.mapLatitude);
                const lon = parseFloat(mapData.mapLongitude);
                const zoom = parseInt(mapData.mapZoom) || 17;

                if (isNaN(lat) || isNaN(lon)) {
                    console.error("[LEAFLET DEBUG] Invalid latitude or longitude:", mapData.mapLatitude, mapData.mapLongitude);
                    mapDiv.innerHTML = "<p style='color:red; padding:10px;'>Error: Invalid map coordinates.</p>";
                    mapDiv.style.border = "2px solid red";
                    return;
                }

                const map = L.map(mapId).setView([lat, lon], zoom);

                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
                    maxZoom: 19,
                    detectRetina: true
                }).addTo(map);

                L.marker([lat, lon]).addTo(map)
                    .bindPopup(`Location: (${lat.toFixed(4)}, ${lon.toFixed(4)})`)
                    .openPopup();

                setTimeout(() => {
                    map.invalidateSize();
                }, 50);

            } catch (e) {
                console.error("[LEAFLET DEBUG] Error during Leaflet map initialization for #" + mapId + ":", e);
                mapDiv.innerHTML = `<p style='color:red; padding:10px;'>Error: Could not load map. ${e.message}</p>`;
                mapDiv.style.border = "2px solid red";
            }
        });

    } else if (mapData.mapQuery && typeof mapData.mapQuery === 'string' && mapData.mapQuery.startsWith('http')) {
        console.warn("[LEAFLET DEBUG] No coordinates. Fallback to iframe for mapQuery URL:", mapData.mapQuery);
        const mapContainer = document.createElement('div');
        mapContainer.className = 'map-container-render message-section';         
        const iframe = document.createElement('iframe');
        iframe.width = '100%';
        iframe.height = '300';         
        iframe.style.border = 'none';
        iframe.src = mapData.mapQuery;
        mapContainer.appendChild(iframe);
        parentContainer.appendChild(mapContainer);
    }
}
