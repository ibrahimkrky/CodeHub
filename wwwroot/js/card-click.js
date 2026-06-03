// Tıklanabilir kartlar: kartın herhangi bir yerine tıklayınca detaya git.
// İçindeki gerçek linklere (yazar, etiket vb.) tıklanırsa onların işlevi korunur.
(function () {
    document.querySelectorAll('.ch-card.clickable').forEach(function (card) {
        card.addEventListener('click', function (e) {
            // Eğer tıklanan öğe bir link/buton ya da onların içindeyse, kartın yönlendirmesini yapma
            if (e.target.closest('a, button, form')) return;
            const href = card.getAttribute('data-href');
            if (href) window.location = href;
        });
    });
})();
