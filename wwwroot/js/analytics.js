// Analiz paneli: Controller'dan JSON çek, Chart.js ile çiz
(function () {
    fetch('/Analytics/Data')
        .then(r => r.json())
        .then(data => {
            // İstatistik kartları
            document.getElementById('statSnippets').innerText = data.totalSnippets;
            document.getElementById('statViews').innerText = data.totalViews;
            document.getElementById('statLikes').innerText = data.totalLikes;

            const textColor = '#8b949e';
            Chart.defaults.color = textColor;
            Chart.defaults.font.family = "'JetBrains Mono', monospace";

            // Pie: dile göre dağılım
            const langCtx = document.getElementById('langChart');
            new Chart(langCtx, {
                type: 'pie',
                data: {
                    labels: data.byLanguage.map(x => x.language),
                    datasets: [{
                        data: data.byLanguage.map(x => x.count),
                        backgroundColor: ['#f0a93b', '#58a6ff', '#3fb950', '#f85149', '#a371f7', '#db61a2', '#39c5cf', '#e3b341']
                    }]
                },
                options: { responsive: true, plugins: { legend: { position: 'bottom' } } }
            });

            // Bar: snippet görüntülenmeleri
            const viewsCtx = document.getElementById('viewsChart');
            new Chart(viewsCtx, {
                type: 'bar',
                data: {
                    labels: data.views.map(x => x.title.length > 16 ? x.title.substring(0,16)+'…' : x.title),
                    datasets: [{
                        label: 'Görüntülenme',
                        data: data.views.map(x => x.views),
                        backgroundColor: '#58a6ff'
                    }]
                },
                options: {
                    responsive: true,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { grid: { color: '#2a3038' } },
                        x: { grid: { color: '#2a3038' } }
                    }
                }
            });
        })
        .catch(err => console.error('Analiz verisi alınamadı:', err));
})();
