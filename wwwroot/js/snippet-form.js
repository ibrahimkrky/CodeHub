// 1) Kod alanında Tab tuşu girinti eklesin (focus kaybetmeden)
(function () {
    const codeArea = document.getElementById('codeArea');
    if (codeArea) {
        codeArea.addEventListener('keydown', function (e) {
            if (e.key === 'Tab') {
                e.preventDefault();
                const start = this.selectionStart, end = this.selectionEnd;
                this.value = this.value.substring(0, start) + '    ' + this.value.substring(end);
                this.selectionStart = this.selectionEnd = start + 4;
            }
        });
    }

    // 2) Dinamik etiket ekleme/silme
    const tagInput = document.getElementById('tagInput');
    const tagsCsv = document.getElementById('tagsCsv');
    const tagBadges = document.getElementById('tagBadges');
    let tags = [];

    // Edit sayfasında hazır gelen etiketleri yükle
    if (tagsCsv && tagsCsv.value) {
        tags = tagsCsv.value.split(',').map(t => t.trim()).filter(Boolean);
        render();
    }

    if (tagInput) {
        tagInput.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ',') {
                e.preventDefault();
                const val = this.value.trim().replace(/,$/, '');
                if (val && !tags.includes(val)) {
                    tags.push(val);
                    render();
                }
                this.value = '';
            }
        });
    }

    function render() {
        tagBadges.innerHTML = '';
        tags.forEach((t, i) => {
            const span = document.createElement('span');
            span.className = 'ch-badge me-1 mb-1 d-inline-block';
            span.style.cursor = 'pointer';
            span.innerHTML = t + ' <i class="bi bi-x"></i>';
            span.onclick = () => { tags.splice(i, 1); render(); };
            tagBadges.appendChild(span);
        });
        tagsCsv.value = tags.join(', ');
    }
})();
