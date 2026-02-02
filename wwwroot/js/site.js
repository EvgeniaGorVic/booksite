// Hover animation
document.querySelectorAll('.book-card').forEach(card => {
    card.addEventListener('mouseenter', () => {
        card.style.transform = 'translateY(-6px)';
    });
    card.addEventListener('mouseleave', () => {
        card.style.transform = 'translateY(0)';
    });
});

// Button feedback
document.querySelectorAll('button').forEach(btn => {
    btn.addEventListener('click', () => {
        btn.style.opacity = '0.8';
        setTimeout(() => btn.style.opacity = '1', 150);
    });
});

// Simple auth UX
document.querySelectorAll('.auth-card input').forEach(input => {
    input.addEventListener('focus', () => {
        input.style.boxShadow = '0 0 0 3px rgba(90,103,255,.2)';
    });
    input.addEventListener('blur', () => {
        input.style.boxShadow = 'none';
    });
});

// Cart mock
function addToCart(id) {
    alert(`Книга #${id} добавлена в корзину`);
}
// Добавьте этот код в main.js
function initAuthForms() {
    // Инициализация переключения форм
    const wrapper = document.getElementById('wrapper');
    if (!wrapper) return;

    const hash = window.location.hash;
    if (hash === '#tosubscribe') {
        wrapper.classList.add('switch');
    }

    // Обработка кликов по ссылкам переключения
    document.addEventListener('click', function (e) {
        if (e.target.matches('a[href="#tosubscribe"], a[href="#tologin"]')) {
            e.preventDefault();
            const href = e.target.getAttribute('href');

            if (href === '#tosubscribe') {
                wrapper.classList.add('switch');
                history.pushState(null, null, '#tosubscribe');
            } else {
                wrapper.classList.remove('switch');
                history.pushState(null, null, '#tologin');
            }
        }
    });

    // Обработка истории браузера
    window.addEventListener('popstate', function () {
        if (window.location.hash === '#tosubscribe') {
            wrapper.classList.add('switch');
        } else {
            wrapper.classList.remove('switch');
        }
    });
}

// Вызовите функцию при загрузке
document.addEventListener('DOMContentLoaded', initAuthForms);