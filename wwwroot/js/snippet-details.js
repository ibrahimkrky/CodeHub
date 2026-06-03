// Kodu panoya kopyala (Clipboard API)
function copyCode() {
    const code = document.querySelector('pre code');
    if (!code) return;
    navigator.clipboard.writeText(code.innerText).then(() => {
        const btn = document.getElementById('copyBtn');
        const original = btn.innerHTML;
        btn.innerHTML = '<i class="bi bi-check2"></i> Kopyalandı';
        btn.classList.add('liked');
        setTimeout(() => { btn.innerHTML = original; btn.classList.remove('liked'); }, 1500);
    });
}

// AJAX beğeni — sayfa yenilenmeden
function toggleStar() {
    const btn = document.getElementById('starBtn');
    const id = btn.getAttribute('data-id');

    fetch('/Snippet/Star/' + id, {
        method: 'POST',
        headers: { 'RequestVerificationToken': getToken() }
    })
    .then(r => {
        if (r.status === 401) { window.location = '/Account/Login'; return null; }
        return r.json();
    })
    .then(data => {
        if (!data) return;
        document.getElementById('likeCount').innerText = data.likeCount;
        btn.classList.toggle('liked', data.liked);
    })
    .catch(err => console.error(err));
}

// AntiForgery token'ı sayfadaki herhangi bir formdan al
function getToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}
