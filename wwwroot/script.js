// DOM Elements
const loginSection = document.getElementById('loginSection');
const signupSection = document.getElementById('signupSection');
const loginForm = document.getElementById('loginForm');
const signupForm = document.getElementById('signupForm');
const showSignupLink = document.getElementById('showSignup');
const showLoginLink = document.getElementById('showLogin');
const alertBox = document.getElementById('alert');
const alertMessage = document.getElementById('alertMessage');

// Check if already logged in
window.addEventListener('load', () => {
    const userData = localStorage.getItem('user');
    if (userData) {
        const user = JSON.parse(userData);
        if (user.role === 'Sender') {
            window.location.href = 'sender-dashboard.html';
        } else if (user.role === 'Handler') {
            window.location.href = 'handler-dashboard.html';
        }
    }
});

// Show alert message
function showAlert(message, type = 'success') {
    alertMessage.textContent = message;
    alertBox.className = `alert ${type}`;
    alertBox.classList.remove('hidden');
    
    if (type !== 'error') {
        setTimeout(() => {
            hideAlert();
        }, 5000);
    }
}

// Hide alert
function hideAlert() {
    alertBox.className = 'alert hidden';
}

// Setup form switching
showSignupLink.addEventListener('click', (e) => {
    e.preventDefault();
    loginSection.classList.add('hidden');
    signupSection.classList.remove('hidden');
});

showLoginLink.addEventListener('click', (e) => {
    e.preventDefault();
    signupSection.classList.add('hidden');
    loginSection.classList.remove('hidden');
});

// Handle Login
loginForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    
    if (!email || !password) {
        showAlert('Please fill in all fields', 'error');
        return;
    }
    
    try {
        const response = await fetch('http://localhost:5000/api/Account/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password }),
            credentials: 'include'
        });
        
        const data = await response.json();
        
        if (response.ok) {
            localStorage.setItem('user', JSON.stringify(data));
            showAlert('Login successful! Redirecting...', 'success');
            
            // Redirect based on user role
            if (data.role === 'Sender') {
                window.location.href = 'sender-dashboard.html';
            } else if (data.role === 'Handler') {
                window.location.href = 'handler-dashboard.html';
            }
        } else {
            showAlert(data.message || 'Invalid email or password', 'error');
        }
    } catch (error) {
        showAlert('Network error. Please try again.', 'error');
    }
});

// Handle Signup
signupForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const name = document.getElementById('signupName').value;
    const email = document.getElementById('signupEmail').value;
    const password = document.getElementById('signupPassword').value;
    const role = document.getElementById('signupRole').value;
    
    if (!name || !email || !password || !role) {
        showAlert('Please fill in all fields', 'error');
        return;
    }
    
    try {
        const response = await fetch('http://localhost:5000/api/User/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name, email, password, role }),
            credentials: 'include'
        });
        
        const data = await response.json();
        
        if (response.ok) {
            localStorage.setItem('user', JSON.stringify(data));
            showAlert('Registration successful! Redirecting...', 'success');
            
            // Redirect based on user role
            if (data.role === 'Sender') {
                window.location.href = 'sender-dashboard.html';
            } else if (data.role === 'Handler') {
                window.location.href = 'handler-dashboard.html';
            }
        } else {
            showAlert(data.message || 'Registration failed', 'error');
        }
    } catch (error) {
        showAlert('Network error. Please try again.', 'error');
    }
});

// Handle dismiss alert button
document.getElementById('dismissAlert').addEventListener('click', hideAlert);


