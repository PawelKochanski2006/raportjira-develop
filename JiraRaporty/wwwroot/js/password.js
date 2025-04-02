﻿// Funkcja wyświetlająca i ukrywająca hasło 
function togglePassword() {
    var passwordInput = document.getElementById('password');
    var toggleButton = document.getElementById('togglePassword');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleButton.textContent = 'Hide';
    } else {
        passwordInput.type = 'password';
        toggleButton.textContent = 'Show';
    }
}