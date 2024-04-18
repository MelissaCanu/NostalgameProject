var stripePublicKey = 'pk_test_51P5jguRs6OMBWVDjj56F6ToNKNG6Z9IZlk7uIPCW9zzfksZ0Qg1AJHMvBMLW6GV9pmREl3lYmERt7APvxxi4OgNI00CvkPWz5b';

var stripe = Stripe(stripePublicKey);
var elements = stripe.elements();

// Crea un'istanza dell'elemento carta
var card = elements.create('card');
card.mount('#card-element');

// Gestisci il submit del form
var form = document.getElementById('payment-form');
form.addEventListener('submit', function (event) {
    event.preventDefault();

    // Imposta il valore di ImportoPagato al valore di CostoAnnuale
    document.getElementById('ImportoPagato').value = document.getElementById('CostoAnnuale').value;

    stripe.createPaymentMethod({
        type: 'card',
        card: card,
    }).then(function (result) {
        if (result.error) {
            // Mostra gli errori nel form
            var errorElement = document.getElementById('card-errors');
            errorElement.textContent = result.error.message;
        } else {
            // Invia il PaymentMethod al tuo server
            stripePaymentMethodHandler(result.paymentMethod);
        }
    });
});

// Invia il PaymentMethod al tuo server
function stripePaymentMethodHandler(paymentMethod) {
    // Inserisci l'ID del PaymentMethod nel form in modo da poterlo inviare al server
    var form = document.getElementById('payment-form');
    var hiddenInput = document.createElement('input');
    hiddenInput.setAttribute('type', 'hidden');
    hiddenInput.setAttribute('name', 'stripeToken'); // Cambia 'paymentMethodId' in 'stripeToken'
    hiddenInput.setAttribute('value', paymentMethod.id);
    form.appendChild(hiddenInput);

    // Invia il form
    form.submit();
}
