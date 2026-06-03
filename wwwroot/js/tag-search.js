// Etiket dizini: canlı arama (live search)
(function () {
    const box = document.getElementById('tagSearch');
    const items = document.querySelectorAll('.tag-item');
    const noResult = document.getElementById('tagNoResult');
    if (!box) return;

    box.addEventListener('input', function () {
        const q = this.value.trim().toLowerCase();
        let visible = 0;
        items.forEach(item => {
            const name = item.getAttribute('data-name') || '';
            const match = name.includes(q);
            item.style.display = match ? '' : 'none';
            if (match) visible++;
        });
        noResult.style.display = visible === 0 ? '' : 'none';
    });
})();
