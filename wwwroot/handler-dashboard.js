// Handler Dashboard JavaScript

// Global variables
let dailyTrendChart = null;
let statusChart = null;
let currentDays = 7;
let userData = null;

// Initialize dashboard on page load
document.addEventListener('DOMContentLoaded', () => {
    // Check if user is logged in
    checkAuth();
    
    // Set up event listeners
    setupEventListeners();
    
    // Load dashboard data
    loadDashboardData(currentDays);
    
    // Load pending scans
    loadPendingScans();
    
    // Load recent activity
    loadRecentActivity();
    
    // Load alerts
    loadAlerts();
});

// Check if user is authenticated and is a handler
function checkAuth() {
    fetch('/api/account/profile', {
        credentials: 'include'
    })
    .then(response => {
        if (!response.ok) {
            window.location.href = '/index.html';
            throw new Error('Not authenticated');
        }
        return response.json();
    })
    .then(data => {
        if (!data.roles || !data.roles.includes('Handler')) {
            window.location.href = '/index.html';
            throw new Error('Not authorized as Handler');
        }
        
        userData = data;
        document.getElementById('userNameDisplay').textContent = data.firstName + ' ' + data.lastName;
        
        // Load handler location
        loadHandlerLocation();
    })
    .catch(error => {
        console.error('Auth check failed:', error);
        // Redirect to login if authentication fails
        window.location.href = '/index.html';
    });
}

// Set up event listeners for dashboard interactions
function setupEventListeners() {
    // Trend days dropdown
    document.querySelectorAll('[data-days]').forEach(item => {
        item.addEventListener('click', (e) => {
            e.preventDefault();
            const days = parseInt(e.target.getAttribute('data-days'));
            currentDays = days;
            document.getElementById('trendDropdown').textContent = `Last ${days} Days`;
            loadDashboardData(days);
        });
    });
    
    // Logout button
    document.getElementById('logoutBtn').addEventListener('click', () => {
        fetch('/api/account/logout', {
            method: 'POST',
            credentials: 'include'
        })
        .then(() => {
            window.location.href = '/index.html';
        })
        .catch(error => console.error('Logout failed:', error));
    });
    
    // Update status button in modal
    document.getElementById('updateStatusBtn').addEventListener('click', updateParcelStatus);
}

// Load handler's location information
function loadHandlerLocation() {
    fetch('/api/handler/location', {
        credentials: 'include'
    })
    .then(response => response.json())
    .then(data => {
        if (data.success && data.data) {
            document.getElementById('locationDisplay').textContent = data.data.locationName;
        }
    })
    .catch(error => console.error('Failed to load location:', error));
}

// Load dashboard statistics
function loadDashboardData(days = 7) {
    fetch(`/api/handler/dashboard?days=${days}`, {
        credentials: 'include'
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            updateDashboardStats(data.data);
            updateDailyTrendChart(data.data.dailyHandledParcels);
            updateStatusChart(data.data.statusCounts);
        }
    })
    .catch(error => console.error('Failed to load dashboard data:', error));
}

// Update dashboard statistics cards
function updateDashboardStats(stats) {
    document.getElementById('parcelsTodayCount').textContent = stats.totalParcelsToday;
    document.getElementById('pendingScansCount').textContent = stats.pendingScans;
    document.getElementById('alertsCount').textContent = stats.totalActiveAlerts;
    
    // Find delivered count
    const deliveredStatus = stats.statusCounts.find(s => s.status === 'Delivered');
    document.getElementById('deliveredTodayCount').textContent = deliveredStatus ? deliveredStatus.count : 0;
}

// Update daily trend chart
function updateDailyTrendChart(dailyData) {
    const ctx = document.getElementById('dailyTrendChart').getContext('2d');
    
    // Destroy previous chart if it exists
    if (dailyTrendChart) {
        dailyTrendChart.destroy();
    }
    
    // Prepare data
    const labels = dailyData.map(item => {
        const date = new Date(item.date);
        return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
    });
    
    const values = dailyData.map(item => item.count);
    
    // Create new chart
    dailyTrendChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Handled Parcels',
                data: values,
                borderColor: '#007bff',
                backgroundColor: 'rgba(0, 123, 255, 0.1)',
                borderWidth: 2,
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            }
        }
    });
}

// Update status distribution chart
function updateStatusChart(statusData) {
    const ctx = document.getElementById('statusChart').getContext('2d');
    
    // Destroy previous chart if it exists
    if (statusChart) {
        statusChart.destroy();
    }
    
    // Prepare data
    const labels = statusData.map(item => item.status);
    const values = statusData.map(item => item.count);
    
    // Define colors for each status
    const colors = {
        'Received': 'rgba(23, 162, 184, 0.8)',
        'In-Transit': 'rgba(255, 193, 7, 0.8)',
        'Out-for-Delivery': 'rgba(0, 123, 255, 0.8)',
        'Delivered': 'rgba(40, 167, 69, 0.8)',
        'Exception': 'rgba(220, 53, 69, 0.8)'
    };
    
    // Get colors based on status
    const backgroundColors = statusData.map(item => colors[item.status] || 'rgba(108, 117, 125, 0.8)');
    
    // Create new chart
    statusChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: backgroundColors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                }
            }
        }
    });
}

// Load parcels pending scan
function loadPendingScans() {
    fetch('/api/handler/pending-scans', {
        credentials: 'include'
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            populatePendingScansTable(data.data);
        }
    })
    .catch(error => console.error('Failed to load pending scans:', error));
}

// Populate the pending scans table
function populatePendingScansTable(parcels) {
    const tableBody = document.getElementById('pendingScansTable');
    tableBody.innerHTML = '';
    
    if (parcels.length === 0) {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="4" class="text-center">No pending scans</td>`;
        tableBody.appendChild(row);
        return;
    }
    
    parcels.forEach(parcel => {
        const row = document.createElement('tr');
        row.className = parcel.isPriority ? 'table-warning' : '';
        
        row.innerHTML = `
            <td>${parcel.trackingNumber}</td>
            <td>${parcel.recipientName}</td>
            <td><span class="badge bg-${getStatusBadgeClass(parcel.status)}">${parcel.status}</span></td>
            <td>
                <button class="btn btn-sm btn-primary update-status-btn" 
                        data-id="${parcel.id}" 
                        data-tracking="${parcel.trackingNumber}"
                        data-recipient="${parcel.recipientName}">
                    Update
                </button>
            </td>
        `;
        
        tableBody.appendChild(row);
    });
    
    // Add event listeners to update status buttons
    document.querySelectorAll('.update-status-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
            const parcelId = btn.getAttribute('data-id');
            const trackingNumber = btn.getAttribute('data-tracking');
            const recipientName = btn.getAttribute('data-recipient');
            
            // Populate modal
            document.getElementById('parcelId').value = parcelId;
            document.getElementById('trackingNumberDisplay').textContent = trackingNumber;
            document.getElementById('recipientDisplay').textContent = recipientName;
            
            // Show modal
            const modal = new bootstrap.Modal(document.getElementById('updateStatusModal'));
            modal.show();
        });
    });
}

// Get appropriate badge class for status
function getStatusBadgeClass(status) {
    switch (status) {
        case 'Received': return 'info';
        case 'In-Transit': return 'warning';
        case 'Out-for-Delivery': return 'primary';
        case 'Delivered': return 'success';
        case 'Exception': return 'danger';
        default: return 'secondary';
    }
}

// Update parcel status
function updateParcelStatus() {
    const parcelId = document.getElementById('parcelId').value;
    const status = document.getElementById('statusSelect').value;
    const notes = document.getElementById('notesInput').value;
    
    if (!status) {
        alert('Please select a status');
        return;
    }
    
    fetch(`/api/handler/parcel/${parcelId}/status`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify({
            status: status,
            notes: notes
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Close modal
            bootstrap.Modal.getInstance(document.getElementById('updateStatusModal')).hide();
            
            // Reset form
            document.getElementById('updateStatusForm').reset();
            
            // Refresh data
            loadDashboardData(currentDays);
            loadPendingScans();
            loadRecentActivity();
            
            // Show success message
            alert('Parcel status updated successfully');
        } else {
            alert('Error: ' + data.message);
        }
    })
    .catch(error => {
        console.error('Failed to update status:', error);
        alert('Failed to update status. Please try again.');
    });
}

// Load recent activity
function loadRecentActivity() {
    fetch('/api/handler/recent-activity', {
        credentials: 'include'
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            populateRecentActivity(data.data);
        }
    })
    .catch(error => console.error('Failed to load recent activity:', error));
}

// Populate recent activity list
function populateRecentActivity(activities) {
    const activityList = document.getElementById('recentActivityList');
    activityList.innerHTML = '';
    
    if (activities.length === 0) {
        activityList.innerHTML = '<li class="list-group-item">No recent activity</li>';
        return;
    }
    
    activities.forEach(activity => {
        const li = document.createElement('li');
        li.className = 'list-group-item';
        
        // Format date
        const date = new Date(activity.timestamp);
        const formattedDate = date.toLocaleString();
        
        li.innerHTML = `
            <div class="d-flex justify-content-between align-items-center">
                <div>
                    <h6 class="mb-1">${activity.trackingNumber} - ${activity.recipientName}</h6>
                    <small class="text-muted">
                        <span class="badge bg-${getStatusBadgeClass(activity.status)}">${activity.action}</span>
                        ${activity.location}
                    </small>
                </div>
                <small class="text-muted">${formattedDate}</small>
            </div>
            ${activity.notes ? `<small class="d-block text-muted">${activity.notes}</small>` : ''}
        `;
        
        activityList.appendChild(li);
    });
}

// Load alerts
function loadAlerts() {
    fetch('/api/handler/alerts', {
        credentials: 'include'
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Update alert count in dashboard
            document.getElementById('alertsCount').textContent = data.data.length;
        }
    })
    .catch(error => console.error('Failed to load alerts:', error));
}
