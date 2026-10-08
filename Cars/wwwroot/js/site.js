document.querySelectorAll('[data-confirm]').forEach(form => {
  form.addEventListener('submit', event => {
    if (!window.confirm(form.dataset.confirm)) event.preventDefault();
  });
});

document.querySelectorAll('[data-toggle-password]').forEach(button => {
  button.addEventListener('click', () => {
    const input = button.closest('.password-wrap')?.querySelector('input');
    if (!input) return;
    input.type = input.type === 'password' ? 'text' : 'password';
    button.textContent = input.type === 'password' ? 'Show' : 'Hide';
    button.setAttribute('aria-label', `${button.textContent} password`);
  });
});

document.querySelectorAll('[data-open-dialog]').forEach(button => {
  button.addEventListener('click', () => document.getElementById(button.dataset.openDialog)?.showModal());
});
document.querySelectorAll('[data-close-dialog]').forEach(button => {
  button.addEventListener('click', () => button.closest('dialog')?.close());
});
document.querySelectorAll('[data-edit-car]').forEach(button => {
  button.addEventListener('click', () => {
    const dialog = document.getElementById('edit-vehicle');
    if (!dialog) return;
    const fields = { id:'id', brand:'brand', model:'model', licensePlate:'plate', year:'year',
      category:'category', transmission:'transmission', seats:'seats', pricePerDay:'price',
      description:'description' };
    for (const [name, key] of Object.entries(fields)) {
      const field = dialog.querySelector(`[name="${name}"]`);
      if (field) field.value = button.dataset[key] || '';
    }
    const thumb = button.closest('tr')?.querySelector('.vehicle-thumb');
    const preview = dialog.querySelector('[data-upload-preview]');
    if (thumb && preview) {
      const current = getComputedStyle(thumb);
      preview.style.backgroundImage = current.backgroundImage;
      preview.style.backgroundSize = current.backgroundSize;
      preview.style.backgroundPosition = current.backgroundPosition;
      preview.hidden = false;
    }
    dialog.querySelector('[name="photo"]').value = '';
    dialog.showModal();
  });
});

document.querySelectorAll('.photo-upload input[type="file"]').forEach(input => {
  input.addEventListener('change', () => {
    const file = input.files?.[0];
    const preview = input.closest('.photo-upload')?.querySelector('[data-upload-preview]');
    input.setCustomValidity('');
    if (!file) {
      if (preview && input.closest('#add-vehicle')) preview.hidden = true;
      return;
    }
    if (file.size > 2 * 1024 * 1024) {
      input.setCustomValidity('Choose a photo that is 2 MB or smaller.');
      input.reportValidity();
      return;
    }
    if (preview) {
      const reader = new FileReader();
      reader.onload = () => {
        preview.style.backgroundImage = `url(${reader.result})`;
        preview.style.backgroundSize = 'cover';
        preview.style.backgroundPosition = 'center';
        preview.hidden = false;
      };
      reader.readAsDataURL(file);
    }
  });
});

const bookingForm = document.querySelector('[data-booking-form]');
if (bookingForm) {
  const pickup = bookingForm.querySelector('[name="Input.PickupDate"]');
  const returned = bookingForm.querySelector('[name="Input.ReturnDate"]');
  const insurance = bookingForm.querySelector('[name="Input.Insurance"]');
  const price = Number(bookingForm.dataset.price || 0);
  const money = value => '₱' + value.toLocaleString('en-PH', { maximumFractionDigits: 0 });
  const now = new Date();
  const today = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
  pickup.min = today;
  returned.min = today;

  const update = () => {
    const from = pickup.value ? new Date(pickup.value + 'T00:00:00') : null;
    const to = returned.value ? new Date(returned.value + 'T00:00:00') : null;
    const days = from && to ? Math.round((to - from) / 86400000) : 0;
    const validDays = days > 0 && days <= 30 ? days : 0;
    const base = price * validDays;
    const insuranceFee = insurance.checked ? 500 : 0;
    document.querySelector('[data-days]').textContent = validDays || '—';
    document.querySelector('[data-base]').textContent = money(base);
    document.querySelector('[data-insurance]').textContent = money(insuranceFee);
    document.querySelector('[data-total]').textContent = money(base + insuranceFee);
    returned.min = pickup.value || today;
  };
  [pickup, returned, insurance].forEach(input => input.addEventListener('change', update));
  update();
}

const adminBooking = document.querySelector('[data-admin-booking]');
if (adminBooking) {
  const steps = [...adminBooking.querySelectorAll('[data-step]')];
  const stepLabels = [...document.querySelectorAll('.booking-steps li')];
  const next = adminBooking.querySelector('[data-next]');
  const prev = adminBooking.querySelector('[data-prev]');
  const submit = adminBooking.querySelector('[data-submit]');
  const pickup = adminBooking.querySelector('[name="Input.PickupDate"]');
  const returned = adminBooking.querySelector('[name="Input.ReturnDate"]');
  const insurance = adminBooking.querySelector('[name="Input.Insurance"]');
  const money = value => '₱' + value.toLocaleString('en-PH', { maximumFractionDigits: 0 });
  const now = new Date();
  const today = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
  pickup.min = today;
  returned.min = today;
  let current = 0;

  const refresh = () => {
    const chosen = adminBooking.querySelector('[name="Input.CarId"]:checked');
    const daily = Number(chosen?.dataset.price || 0);
    const from = pickup.value ? new Date(pickup.value + 'T00:00:00') : null;
    const to = returned.value ? new Date(returned.value + 'T00:00:00') : null;
    const rawDays = from && to ? Math.round((to - from) / 86400000) : 0;
    const days = rawDays > 0 && rawDays <= 30 ? rawDays : 0;
    const base = daily * days;
    const fee = insurance.checked ? 500 : 0;
    document.querySelector('[data-summary-car]').textContent = chosen?.dataset.name || 'Choose a vehicle';
    document.querySelector('[data-summary-category]').textContent = chosen?.dataset.category || 'Available cars only';
    const summaryPhoto = document.querySelector('[data-summary-photo]');
    const chosenPhoto = chosen?.closest('.booking-car-option')?.querySelector('.booking-option-photo');
    summaryPhoto.className = `detail-vehicle-image photo-${chosen?.dataset.slot || 1}`;
    summaryPhoto.style.backgroundImage = chosenPhoto?.style.backgroundImage || '';
    summaryPhoto.style.backgroundSize = chosenPhoto?.style.backgroundSize || '';
    summaryPhoto.style.backgroundPosition = chosenPhoto?.style.backgroundPosition || '';
    document.querySelector('[data-admin-days]').textContent = days || '—';
    document.querySelector('[data-admin-rate]').textContent = money(daily);
    document.querySelector('[data-admin-base]').textContent = money(base);
    document.querySelector('[data-admin-insurance]').textContent = money(fee);
    document.querySelector('[data-admin-total]').textContent = money(base + fee);
    returned.min = pickup.value || today;
    const customer = adminBooking.querySelector('[name="Input.FullName"]').value || '—';
    const dates = pickup.value && returned.value ? `${pickup.value} → ${returned.value}` : '—';
    adminBooking.querySelector('[data-review-car]').textContent = chosen?.dataset.name || '—';
    adminBooking.querySelector('[data-review-customer]').textContent = customer;
    adminBooking.querySelector('[data-review-dates]').textContent = dates;
    adminBooking.querySelector('[data-review-days]').textContent = days || '—';
    adminBooking.querySelector('[data-review-total]').textContent = money(base + fee);
  };
  const show = index => {
    current = index;
    steps.forEach((step, i) => { step.hidden = i !== current; });
    stepLabels.forEach((label, i) => { label.classList.toggle('active', i === current); label.classList.toggle('complete', i < current); });
    prev.hidden = current === 0;
    next.hidden = current === steps.length - 1;
    submit.hidden = current !== steps.length - 1;
    refresh();
  };
  next.addEventListener('click', () => {
    for (const field of steps[current].querySelectorAll('[required]')) {
      if (!field.checkValidity()) { field.reportValidity(); return; }
    }
    if (current === 2) {
      const days = Math.round((new Date(returned.value + 'T00:00:00') - new Date(pickup.value + 'T00:00:00')) / 86400000);
      if (pickup.value < today || days < 1 || days > 30) { window.alert('Choose a future pickup and a rental length from 1 to 30 days.'); return; }
    }
    show(Math.min(current + 1, steps.length - 1));
  });
  prev.addEventListener('click', () => show(Math.max(current - 1, 0)));
  adminBooking.querySelector('[data-existing-customer]').addEventListener('change', event => {
    const option = event.target.selectedOptions[0];
    if (!option?.value) return;
    adminBooking.querySelector('[name="Input.FullName"]').value = option.dataset.name || '';
    adminBooking.querySelector('[name="Input.Phone"]').value = option.dataset.phone || '';
    adminBooking.querySelector('[name="Input.LicenseNumber"]').value = option.dataset.license || '';
    refresh();
  });
  adminBooking.addEventListener('change', refresh);
  adminBooking.addEventListener('input', refresh);
  show(0);
}
