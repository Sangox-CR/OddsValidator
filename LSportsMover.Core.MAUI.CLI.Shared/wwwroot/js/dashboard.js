document.addEventListener('DOMContentLoaded', () => {
    // Simulated API call for weather data
    function updateWeather() {
        const weatherIcons = ['fas fa-sun', 'fas fa-cloud-sun', 'fas fa-cloud', 'fas fa-cloud-showers-heavy', 'fas fa-snowflake'];
        const descriptions = ['Sunny', 'Partly Cloudy', 'Cloudy', 'Rainy', 'Snowy'];
        const randomIndex = Math.floor(Math.random() * weatherIcons.length);
        const temperature = Math.floor(Math.random() * 30) + 10; // Random temperature between 10°C and 40°C

        document.querySelector('.weather-icon i').className = weatherIcons[randomIndex];
        document.querySelector('.weather-temp').textContent = `${temperature}°C`;
        document.querySelector('.weather-description').textContent = descriptions[randomIndex];
    }

    updateWeather();
    setInterval(updateWeather, 60000); // Update weather every minute

    // Revenue Chart
    const revenueCtx = document.getElementById('revenueChart').getContext('2d');
    new Chart(revenueCtx, {
        type: 'line',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            datasets: [{
                label: 'Revenue',
                data: [5000, 7000, 6500, 8000, 9500, 11000, 12000, 11500, 13000, 14500, 13500, 15000],
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value, index, values) {
                            return '$' + value;
                        }
                    }
                }
            }
        }
    });

    // Demographics Chart
    const demographicsCtx = document.getElementById('demographicsChart').getContext('2d');
    new Chart(demographicsCtx, {
        type: 'doughnut',
        data: {
            labels: ['18-24', '25-34', '35-44', '45-54', '55+'],
            datasets: [{
                data: [15, 30, 25, 18, 12],
                backgroundColor: [
                    'rgb(255, 99, 132)',
                    'rgb(54, 162, 235)',
                    'rgb(255, 206, 86)',
                    'rgb(75, 192, 192)',
                    'rgb(153, 102, 255)'
                ]
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'right',
                },
                title: {
                    display: true,
                    text: 'User Age Distribution'
                }
            }
        }
    });
});

window.LoadTerminal = () => {
    /*Terminal */
    const outputArea = document.getElementById('terminalOutput');
    const inputField = document.getElementById('terminalInput');

    // Function to append text to the output area
    window.PrintToTerminal = (text) => {
        outputArea.value += text + '\n';
        outputArea.scrollTop = outputArea.scrollHeight; // Scroll to bottom
    }

    // Handle input on Enter key press
    inputField.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            const command = inputField.value.trim();
            PrintToTerminal(`> ${command}`); // Echo the command

            // Basic command handling example
            if (command === 'help') {
                PrintToTerminal('Available commands: help, echo <text>, clear');
            } else if (command.startsWith('echo ')) {
                PrintToTerminal(command.substring(5));
            } else if (command === 'clear' || command === 'cls') {
                outputArea.value = '';
            } else {
                PrintToTerminal(`Command not found: ${command}`);
            }

            inputField.value = ''; // Clear the input field
        }
    });

    // Initial welcome message
    PrintToTerminal('Welcome to the simulated terminal!');
    PrintToTerminal('Type "help" for a list of commands.');
};



