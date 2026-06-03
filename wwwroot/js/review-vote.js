// Yorum oylama — AJAX, sayfa yenilenmeden (StackOverflow tarzı toggle)
function vote(commentId, value) {
    fetch('/Review/Vote', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': getToken()
        },
        body: 'commentId=' + commentId + '&value=' + value
    })
    .then(r => {
        if (r.status === 401) { window.location = '/Account/Login'; return null; }
        return r.json();
    })
    .then(data => {
        if (!data) return;
        const ctrl = document.querySelector('.vote-controls[data-comment="' + commentId + '"]');
        if (ctrl) ctrl.querySelector('.vote-total').innerText = data.total;

        // Aktif oy yönünü vurgula (myVote: 1 yukarı, -1 aşağı, 0 oy yok)
        const up = document.getElementById('up-' + commentId);
        const down = document.getElementById('down-' + commentId);
        if (up) up.classList.toggle('voted', data.myVote === 1);
        if (down) down.classList.toggle('voted', data.myVote === -1);
    })
    .catch(err => console.error(err));
}

// Yorum düzenleme: metni gizle, formu göster
function startEdit(commentId) {
    document.getElementById('body-' + commentId).style.display = 'none';
    document.getElementById('edit-' + commentId).style.display = 'block';
}

// Düzenlemeyi iptal et: formu gizle, metni geri göster
function cancelEdit(commentId) {
    document.getElementById('edit-' + commentId).style.display = 'none';
    document.getElementById('body-' + commentId).style.display = 'block';
}

function getToken() {
    const el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
}
