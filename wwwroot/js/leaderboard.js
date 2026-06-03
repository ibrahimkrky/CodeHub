// Liderlik tablosu: sütun başlığına tıklayınca sırala
(function () {
    const table = document.getElementById('leaderboardTable');
    if (!table) return;
    const tbody = table.querySelector('tbody');
    const headers = table.querySelectorAll('th[data-col]');
    let asc = {};

    headers.forEach(th => {
        th.addEventListener('click', function () {
            const col = th.getAttribute('data-col');
            const type = th.getAttribute('data-type');
            asc[col] = !asc[col];
            const rows = Array.from(tbody.querySelectorAll('tr'));

            rows.sort((a, b) => {
                let av, bv;
                if (type === 'num') {
                    av = parseInt(a.querySelector('[data-' + col + ']').getAttribute('data-' + col));
                    bv = parseInt(b.querySelector('[data-' + col + ']').getAttribute('data-' + col));
                } else {
                    av = a.querySelector('[data-' + col + ']').getAttribute('data-' + col).toLowerCase();
                    bv = b.querySelector('[data-' + col + ']').getAttribute('data-' + col).toLowerCase();
                }
                if (av < bv) return asc[col] ? -1 : 1;
                if (av > bv) return asc[col] ? 1 : -1;
                return 0;
            });

            // Sıralanmış satırları yeniden ekle ve rütbe numarasını güncelle
            rows.forEach((row, i) => {
                tbody.appendChild(row);
                const rankCell = row.querySelector('td:first-child');
                rankCell.innerHTML = (i + 1);
            });
        });
    });
})();
