// API Base URLs
const API_URL = 'http://localhost:5000/api';
const PARCEL_URL = `${API_URL}/parcel`;
const AUTH_URL = `${API_URL}/account`;

// DOM Elements
const userNameElement = document.getElementById('userName');
const logoutBtn = document.getElementById('logoutBtn');
const navItems = document.querySelectorAll('.sidebar li');
const sections = document.querySelectorAll('.content-section');
const parcelsList = document.getElementById('parcelsList');
const createParcelForm = document.getElementById('createParcelForm');
const alertBox = document.getElementById('alert');

// Show alert message
function showAlert(message, type = 'success') {
    alertBox.textContent = message;
    alertBox.className = `alert ${type} visible`;
    
    setTimeout(() => {
        alertBox.className = 'alert';
    }, 5000);
}

// Format date
function formatDate(dateString) {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleString();
}

// Basic fetch wrapper with error handling
async function fetchAPI(url, options = {}) {
    // Always include credentials for cookie handling
    options.credentials = 'include';
    
    try {
        console.log('Making API request to:', url, options);
        const response = await fetch(url, options);
        console.log('Response status:', response.status);
        
        if (response.status === 401) {
            // Unauthorized - redirect to login
            console.log('Unauthorized access - redirecting to login');
            localStorage.removeItem('user');
            window.location.href = 'index.html';
            return null;
        }
        
        // Try to parse JSON response
        let data;
        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            data = await response.json();
            console.log('Response data:', data);
        } else {
            console.log('Response is not JSON');
            throw new Error('Invalid response format from server');
        }
        
        if (!response.ok) {
            throw new Error(data.message || `Server error: ${response.status}`);
        }
        
        return data;
    } catch (error) {
        console.error(`Error fetching ${url}:`, error);
        showAlert(error.message || 'Network error. Please try again.', 'error');
        return null;
    }
}

// Load user's parcels
async function loadParcels() {
    console.log('Loading parcels...');
    
    if (!parcelsList) {
        console.error('parcelsList element not found in the DOM');
        showAlert('Error: Cannot display parcels - DOM element not found', 'error');
        return;
    }
    
    parcelsList.innerHTML = '<div class="loading">Loading your parcels...</div>';
    
    try {
        console.log('Fetching parcels from API...');
        const data = await fetchAPI(`${PARCEL_URL}/mine`);
        console.log('Parcels response:', data);
        
        if (!data) {
            console.error('No data returned from parcels API');
            parcelsList.innerHTML = `
                <div class="error">
                    <p>Failed to load parcels: No data returned</p>
                    <button onclick="loadParcels()">Try Again</button>
                </div>
            `;
            return;
        }
        
        // Handle empty parcels list
        if (!data.data || data.data.length === 0) {
            console.log('No parcels found for user');
            parcelsList.innerHTML = `
                <div class="empty-state">
                    <p>You don't have any parcels yet.</p>
                    <button onclick="document.querySelector('[data-section=create-parcel]').click()">
                        Create Your First Parcel
                    </button>
                </div>
            `;
            return;
        }
        
        console.log(`Found ${data.data.length} parcels`);
    } catch (error) {
        console.error('Error loading parcels:', error);
        parcelsList.innerHTML = `
            <div class="error">
                <p>Error loading parcels: ${error.message || 'Unknown error'}</p>
                <button onclick="loadParcels()">Try Again</button>
            </div>
        `;
    }
    
    // Create HTML for each parcel
    parcelsList.innerHTML = '';
    data.data.forEach(parcel => {
        const parcelElement = document.createElement('div');
        parcelElement.className = 'parcel-item';
        parcelElement.innerHTML = `
            <div class="parcel-header">
                <span class="tracking-number">Tracking #: ${parcel.trackingNumber || 'N/A'}</span>
                <span class="status">${parcel.status || 'Processing'}</span>
            </div>
            <div class="parcel-details">
                <p><strong>To:</strong> ${parcel.recipientName}</p>
                <p><strong>Address:</strong> ${parcel.recipientAddress}</p>
                <p><strong>Created:</strong> ${formatDate(parcel.createdAt)}</p>
                <p><strong>Weight:</strong> ${parcel.weight} kg</p>
                ${parcel.specialInstructions ? `
                    <p><strong>Instructions:</strong> ${parcel.specialInstructions}</p>
                ` : ''}
            </div>
        `;
        parcelsList.appendChild(parcelElement);
    });
}

// Handle navigation
navItems.forEach(item => {
    item.addEventListener('click', () => {
        // Update active navigation
        navItems.forEach(nav => nav.classList.remove('active'));
        item.classList.add('active');
        
        // Update active section
        const targetSection = item.getAttribute('data-section');
        sections.forEach(section => {
            section.classList.toggle('active', section.id === targetSection);
        });
        
        // Load parcels when switching to my-parcels section
        if (targetSection === 'my-parcels') {
            loadParcels();
        }
    });
});

// Handle create parcel form submission
createParcelForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    // Simple validation
    const recipientName = document.getElementById('recipientName').value;
    const recipientAddress = document.getElementById('recipientAddress').value;
    const weight = document.getElementById('weight').value;
    const specialInstructions = document.getElementById('specialInstructions').value;
    
    if (!recipientName || !recipientAddress || !weight) {
        showAlert('Please fill in all required fields', 'error');
        return;
    }
    
    // Prepare form data
    const parcelData = {
        recipientName,
        recipientAddress,
        weight: parseFloat(weight),
        specialInstructions
    };
    
    try {
        // Submit the form
        console.log('Sending parcel data:', parcelData);
        const data = await fetchAPI(PARCEL_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(parcelData)
        });
        
        if (data && data.success) {
            showAlert('Parcel created successfully!');
            createParcelForm.reset();
            
            // Switch to My Parcels tab
            document.querySelector('[data-section=my-parcels]').click();
        } else {
            showAlert('Failed to create parcel: ' + (data?.message || 'Unknown error'), 'error');
        }
    } catch (error) {
        console.error('Error creating parcel:', error);
        showAlert(`Failed to create parcel: ${error.message}`, 'error');
    }
});

// Handle logout
logoutBtn.addEventListener('click', async () => {
    try {
        await fetch(`${AUTH_URL}/logout`, {
            method: 'POST',
            credentials: 'include'
        });
        localStorage.removeItem('user');
        window.location.href = 'index.html';
    } catch (error) {
        console.error('Logout error:', error);
    }
});

// Check if user is authenticated
async function checkAuth() {
    console.log('Starting authentication check');
    
    try {
        // Get user from localStorage
        const userData = localStorage.getItem('user');
        console.log('User data from localStorage:', userData ? 'Found' : 'Not found');
        
        if (!userData) {
            console.warn('No user data in localStorage, redirecting to login');
            window.location.href = 'index.html';
            return;
        }
        
        // Verify DOM elements are available
        if (!userNameElement) {
            console.error('userNameElement not found in the DOM');
            showAlert('Error: User name element not found', 'error');
        }
        
        // Parse user data
        const user = JSON.parse(userData);
        console.log('User data parsed successfully:', { 
            id: user.id, 
            email: user.email, 
            name: user.name, 
            role: user.role 
        });
        
        // Display user name if the element exists
        if (userNameElement) {
            userNameElement.textContent = user.name || user.email;
        }
        
        // Check if we have the necessary role
        if (user.role !== 'Sender') {
            console.warn(`User has role "${user.role}" but this dashboard is for Senders`);
            showAlert('Access denied: This dashboard is only for Sender users', 'error');
            setTimeout(() => window.location.href = 'index.html', 2000);
            return;
        }
        
        // Skip backend verification for now to troubleshoot UI issues
        console.log('Loading parcels without backend verification');
        loadParcels();
        
        // Enable session verification with the backend
        console.log('Verifying session with backend');
        // Check if session is valid with backend (in the background)
        fetch(`${AUTH_URL}/verify`, {
            credentials: 'include'
        }).then(response => {
            if (!response.ok) {
                console.warn('Session verification failed:', response.status);
                // We don't throw here as we've already loaded parcels
            }
            return response.json();
        }).then(data => {
            console.log('Session verification response:', data);
        }).catch(error => {
            console.error('Session verification error:', error);
        });
    } catch (error) {
        console.error('Auth check failed:', error);
        showAlert('Authentication error: ' + error.message, 'error');
        // Don't redirect immediately to help with debugging
        setTimeout(() => {
            localStorage.removeItem('user');
            window.location.href = 'index.html';
        }, 3000);
    }
}


// Start the app
checkAuth();
