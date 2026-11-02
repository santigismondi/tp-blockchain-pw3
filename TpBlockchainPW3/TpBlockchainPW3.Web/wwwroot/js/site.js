// Comportamientos chicos compartidos por todas las vistas.

// "2.4 MB", "830 KB"...
function formatearTamanio(bytes) {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(0) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
}

// Zona para soltar archivos (.dc-drop): resalta el recuadro al arrastrar y, al elegir
// un archivo, muestra una pastilla con su nombre, su tamaño y una ✕ para quitarlo.
document.querySelectorAll('.dc-drop').forEach(function (zona) {
    var input = zona.querySelector('input[type="file"]');
    if (!input) return;

    var nombre = zona.querySelector('.dc-drop-name');
    var tamanio = zona.querySelector('.dc-drop-size');
    var quitar = zona.querySelector('.dc-drop-clear');

    function mostrarArchivo() {
        var archivo = input.files && input.files[0];
        zona.classList.toggle('has-file', !!archivo);
        if (nombre) nombre.textContent = archivo ? archivo.name : '';
        if (tamanio) tamanio.textContent = archivo ? formatearTamanio(archivo.size) : '';
    }

    input.addEventListener('change', mostrarArchivo);

    if (quitar) {
        quitar.addEventListener('click', function () {
            input.value = '';
            mostrarArchivo();
            input.focus();
        });
    }

    ['dragenter', 'dragover'].forEach(function (evento) {
        zona.addEventListener(evento, function () { zona.classList.add('is-dragover'); });
    });
    ['dragleave', 'drop'].forEach(function (evento) {
        zona.addEventListener(evento, function () { zona.classList.remove('is-dragover'); });
    });

    mostrarArchivo();
});

// Formularios con data-cargando="Texto…": al enviarse (y si pasan la validación)
// el botón se deshabilita y muestra un spinner, así no parece que la página se colgó.
document.querySelectorAll('form[data-cargando]').forEach(function (form) {
    var boton = form.querySelector('button[type="submit"]');
    if (!boton) return;
    var original = boton.innerHTML;

    form.addEventListener('submit', function () {
        if (window.jQuery && jQuery(form).valid && !jQuery(form).valid()) return;
        boton.disabled = true;
        boton.innerHTML = '<span class="spinner-border spinner-border-sm" aria-hidden="true"></span> ' + form.dataset.cargando;
    });

    // Si el usuario vuelve con "Atrás", el navegador puede restaurar el botón deshabilitado
    window.addEventListener('pageshow', function () {
        boton.disabled = false;
        boton.innerHTML = original;
    });
});

// Botones para copiar hashes y códigos: <button class="dc-copy" data-copiar="texto">
document.addEventListener('click', function (e) {
    var boton = e.target.closest('[data-copiar]');
    if (!boton || !navigator.clipboard) return;

    navigator.clipboard.writeText(boton.dataset.copiar).then(function () {
        var icono = boton.querySelector('i');
        boton.classList.add('is-copied');
        if (icono) icono.className = 'bi bi-check2';
        setTimeout(function () {
            boton.classList.remove('is-copied');
            if (icono) icono.className = 'bi bi-copy';
        }, 1500);
    });
});
