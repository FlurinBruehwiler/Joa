#![cfg_attr(
all(not(debug_assertions), target_os = "windows"),
windows_subsystem = "windows"
)]

use tauri::Manager;
use window_shadows::set_shadow;

fn main() {
    tauri::Builder::default()
        .setup(|app| {
            //Enable Shadows
            let window = app.get_window("main").unwrap();
            set_shadow(&window, true).expect("Unsupported platform!");


            Ok(())
        })
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
