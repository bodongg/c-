# AUTODOK visual system

The supplied screenshot is the visual authority for the customer catalog. Use a near-black warm background, dark cards, bright orange for prices and main actions, compact photo-led vehicle cards, and a four-column desktop grid that becomes two and then one column on narrower screens.

The user supplied six additional screenshots that define the admin layout: a split photographic login, fixed sidebar and top search, dashboard analytics, vehicle table with add drawer, stepped booking, reservations table with details pane, and payments table with details pane. Their blue/white palette is replaced throughout with warm charcoal, near black, and orange. Reports, Settings, and support links are omitted at the user's request.

The catalog is the primary customer visual moment. Details and checkout extend the same palette with larger car imagery, restrained panels, and clear form hierarchy. The admin area uses the same materials but prioritizes readable tables, status badges, and quick actions.

Colors are defined as CSS custom properties in `wwwroot/css/site.css`; admin layouts are refined in `wwwroot/css/admin.css`. Car photos are bundled in `wwwroot/images/car-sheet.png`, an AI-generated eight-cell sheet inspired by the reference. The split-login image is `wwwroot/images/login-fleet.png`. The cars and sample prices are illustrative classroom data.
