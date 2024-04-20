var stripe;
var cardElement;
document.addEventListener('DOMContentLoaded', function () {
    stripe = Stripe('pk_test_51P5jguRs6OMBWVDjj56F6ToNKNG6Z9IZlk7uIPCW9zzfksZ0Qg1AJHMvBMLW6GV9pmREl3lYmERt7APvxxi4OgNI00CvkPWz5b');

    var elements = stripe.elements();
    cardElement = elements.create('card');
    cardElement.mount('#card-element');
});


var form = document.getElementById('payment-form');
form.addEventListener('submit', function (event) {
    event.preventDefault();

    stripe.createPaymentMethod('card', cardElement).then(function (result) {
        if (result.error) {
            console.log('Errore nella creazione del metodo di pagamento:', result.error);
            var errorElement = document.getElementById('card-errors');
            errorElement.textContent = result.error.message;
        } else {
            console.log('Metodo di pagamento creato con successo:', result.paymentMethod);
            stripePaymentMethodHandler(result.paymentMethod);
        }
    });
});

// Invia il PaymentMethod al tuo server
function stripePaymentMethodHandler(paymentMethod) {
    // Crea un oggetto con i dati del form
    var data = {
        StripeEmail: document.getElementById('stripeEmail').value,
        PaymentMethodId: paymentMethod.id,
        IdNoleggio: document.querySelector('input[name="IdNoleggio"]').value
    };

    console.log('Dati da inviare al server:', data);


    console.log('Invio richiesta con JSON');

    // Invia i dati al server come JSON
    fetch('/Noleggi/Charge', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
        .then(response => {
            console.log('Risposta dal server:', response);
            return response.json();
        })
        .then(data => console.log('Dati ricevuti dal server:', data))
        .catch(error => console.error('Errore:', error));
}
