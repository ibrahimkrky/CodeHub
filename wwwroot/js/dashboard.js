// Dashboard: arama metni + dil filtresi birlikte (client-side filtering)
(function () {
    const box = document.getElementById('searchBox');
    const items = document.querySelectorAll('.snippet-item');
    const noResult = document.getElementById('noResult');
    const langButtons = document.querySelectorAll('.lang-btn');

    let query = '';
    let activeLang = '';   // boş = tüm diller

    function applyFilters() {
        let visible = 0;
        items.forEach(item => {
            const title = item.getAttribute('data-title') || '';
            const lang = item.getAttribute('data-language') || '';
            const matchText = title.includes(query);
            const matchLang = activeLang === '' || lang === activeLang;
            const show = matchText && matchLang;
            item.style.display = show ? '' : 'none';
            if (show) visible++;
        });
        if (noResult) noResult.style.display = visible === 0 ? '' : 'none';
    }

    // Arama kutusu
    if (box) {
        box.addEventListener('input', function () {
            query = this.value.trim().toLowerCase();
            applyFilters();
        });
    }

    // Dil filtre butonları
    langButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            activeLang = this.getAttribute('data-lang') || '';
            // Aktif butonu vurgula
            langButtons.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            applyFilters();
        });
    });
})();
