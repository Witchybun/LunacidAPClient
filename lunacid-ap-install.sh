#!/usr/bin/env bash
set -euo pipefail

# ╔═════════════════════════════════════════════╗
# ║           LunacidAPClient Installer          ║
# ║   Auto-installs and configures Lunacid AP    ║
# ╚═════════════════════════════════════════════╝

# ──────────────────────────────────────────────
# Configuration
# ──────────────────────────────────────────────
APPID="1745510"
DEFAULT_GAME_DIR="$HOME/.local/share/Steam/steamapps/common/Lunacid"
DEFAULT_PROTON_PREFIX="$HOME/.local/share/Steam/steamapps/compatdata/$APPID/pfx"
API_URL="https://api.github.com/repos/Witchybun/LunacidAPClient/releases"
MIN_VERSION="0.8.9"

# ──────────────────────────────────────────────
# UI Formatting Helpers
# ──────────────────────────────────────────────
BOLD=$(tput bold)
RESET=$(tput sgr0)
GREEN=$(tput setaf 2)
YELLOW=$(tput setaf 3)
CYAN=$(tput setaf 6)
RED=$(tput setaf 1)
CHECK="${GREEN}✔${RESET}"
WARN="${YELLOW}⚠${RESET}"
ERROR="${RED}✖${RESET}"
INFO="${CYAN}ℹ${RESET}"

log_info() { echo -e "${INFO} ${BOLD}$1${RESET}" >&2; }
log_success() { echo -e "${CHECK} ${BOLD}$1${RESET}" >&2; }
log_warn() { echo -e "${WARN} ${BOLD}$1${RESET}" >&2; }
log_error() { echo -e "${ERROR} ${BOLD}$1${RESET}" >&2; }

# ──────────────────────────────────────────────
# Prefix Finder
# ──────────────────────────────────────────────
find_wine_prefix_for_game() {
    local game_name="$1"
    local compatdata_dir="$HOME/.local/share/Steam/steamapps/compatdata"
    local manifests_dir="$HOME/.local/share/Steam/steamapps"

    log_info "Searching for a Wine/Proton prefix for: ${game_name}"
    local prefix=""

    # 1 Check direct Proton AppID path
    if [ -d "$compatdata_dir/$APPID/pfx" ]; then
        log_success "Found Proton prefix (AppID ${APPID}): $compatdata_dir/$APPID/pfx"
        echo "$compatdata_dir/$APPID/pfx"
        return 0
    fi

    # 2 Scan manifests for matching game name
    if [ -d "$manifests_dir" ]; then
        while IFS= read -r -d '' manifest; do
            local appname id
            appname=$(grep -i '"name"' "$manifest" | head -n1 | sed -E 's/.*"name"[[:space:]]*"([^"]+)".*/\1/')
            id=$(basename "$manifest" | grep -o '[0-9]\+')
            if [[ "${appname,,}" == *"${game_name,,}"* ]] && [ -d "$compatdata_dir/$id/pfx" ]; then
                log_success "Found Proton prefix by game name: $compatdata_dir/$id/pfx"
                echo "$compatdata_dir/$id/pfx"
                return 0
            fi
        done < <(find "$manifests_dir" -maxdepth 1 -type f -name "appmanifest_*.acf" -print0)
    fi

    # 3 Search non-Proton Wine prefixes
    log_info "Searching non-Proton Wine prefixes..."
    local search_dirs=(
        "$HOME/.wine"
        "$HOME/wineprefixes"
        "$HOME/.local/share/wineprefixes"
        "$HOME/Games"
        "$HOME/.var/app"
    )

    for base in "${search_dirs[@]}"; do
        [ -d "$base" ] || continue
        while IFS= read -r -d '' pfx; do
            if [ -d "$pfx/drive_c" ]; then
                if find "$pfx/drive_c" -maxdepth 3 -type d -iname "*Lunacid*" | grep -q .; then
                    log_success "Found Wine prefix containing Lunacid: $pfx"
                    echo "$pfx"
                    return 0
                fi
            fi
        done < <(find "$base" -maxdepth 3 -type d \( -name "pfx" -o -name ".wine" \) -print0 2>/dev/null)
    done

    log_warn "Could not automatically locate a Wine/Proton prefix for Lunacid."
    return 1
}

# ──────────────────────────────────────────────
# CLI Argument Parsing
# ──────────────────────────────────────────────
GAME_DIR=""
PROTON_PREFIX=""

while [[ $# -gt 0 ]]; do
    case "$1" in
        -g|--game) GAME_DIR="$2"; shift 2 ;;
        -p|--prefix) PROTON_PREFIX="$2"; shift 2 ;;
        -h|--help)
            cat <<EOF
Usage: $(basename "$0") [OPTIONS]

Options:
  -g, --game <path>     Path to Lunacid game directory
  -p, --prefix <path>   Path to Wine/Proton prefix (only needed for winhttp override)
  -h, --help            Show this help message

Example:
  $(basename "$0") -g "$HOME/.local/share/Steam/steamapps/common/Lunacid"
EOF
            exit 0
            ;;
        *) log_error "Unknown option: $1"; exit 1 ;;
    esac
done

# ──────────────────────────────────────────────
# Game Directory Validation
# ──────────────────────────────────────────────
if [ -z "$GAME_DIR" ]; then
    GAME_DIR="$DEFAULT_GAME_DIR"
fi

if [ ! -d "$GAME_DIR" ]; then
    if command -v zenity &>/dev/null; then
        GAME_DIR=$(zenity --file-selection --directory --title="Select the Lunacid game directory")
    elif command -v kdialog &>/dev/null; then
        GAME_DIR=$(kdialog --getexistingdirectory "$HOME" "Select the Lunacid game directory")
    else
        read -rp "Enter path to Lunacid game directory: " GAME_DIR < /dev/tty
    fi
fi

if [ ! -f "$GAME_DIR/LUNACID.exe" ] && [ ! -f "$GAME_DIR/LUNACID" ]; then
    log_error "$GAME_DIR does not contain the Lunacid executable."
    exit 1
fi

log_success "Using game directory: $GAME_DIR"

# ──────────────────────────────────────────────
# Plugin Management
# ──────────────────────────────────────────────
PLUGIN_DIR="$GAME_DIR/BepInEx/plugins/LunacidAP"
if [ -d "$PLUGIN_DIR" ]; then
    echo ""
    log_info "LunacidAPClient Manager"
    echo "1) Install/Update LunacidAPClient"
    echo "2) Uninstall LunacidAPClient"
    read -rp "Select an option [1/2]: " ACTION < /dev/tty
    if [ "$ACTION" = "2" ]; then
        log_info "Removing LunacidAPClient..."
        rm -rf "$PLUGIN_DIR"
        log_success "Uninstall completed."
        exit 0
    fi
fi

# ──────────────────────────────────────────────
# Apply winhttp Override
# ──────────────────────────────────────────────
echo ""
log_info "LunacidAPClient requires the Wine DLL override: winhttp=native,builtin"
echo "Optional manual method (if this step doesn't work for some reason):"
echo "1 Right-click Lunacid in Steam → Properties → Launch Options →"
echo "  WINEDLLOVERRIDES=\"winhttp.dll=n,b\" %command%"
echo "2 Run protontricks → select Lunacid → (might take a while, ignore errors as long as it doesn't exit) Select the default wineprefix → Run winecfg → Libraries → New override for library → winhttp → Add → Apply"
echo ""

read -rp "Apply the override automatically now? [Y/n] " REPLY < /dev/tty
REPLY=${REPLY:-Y}

if [[ "$REPLY" =~ ^[Yy]$ ]]; then
    if [ -z "$PROTON_PREFIX" ]; then
        PROTON_PREFIX=$(find_wine_prefix_for_game "Lunacid" || true)
    fi

    if [ -z "$PROTON_PREFIX" ] || [ ! -d "$PROTON_PREFIX" ]; then
        log_warn "Prefix not found automatically."
        if command -v zenity &>/dev/null; then
            PROTON_PREFIX=$(zenity --file-selection --directory --title="Select your Proton/Wine prefix directory")
        elif command -v kdialog &>/dev/null; then
            PROTON_PREFIX=$(kdialog --getexistingdirectory "$HOME" "Select your Proton/Wine prefix directory")
        else
            read -rp "Enter path to Proton/Wine prefix: " PROTON_PREFIX < /dev/tty
        fi
    fi

    if [ ! -d "$PROTON_PREFIX" ]; then
        log_error "Invalid prefix directory: $PROTON_PREFIX"
        exit 1
    fi

    log_info "Using prefix: $PROTON_PREFIX"

    REG_FILE="$PROTON_PREFIX/winhttp_override.reg"
    cat > "$REG_FILE" <<EOF
REGEDIT4

[HKEY_CURRENT_USER\\Software\\Wine\\DllOverrides]
"winhttp"="native,builtin"
EOF
    WINEPREFIX="$PROTON_PREFIX" wine regedit "$REG_FILE" >/dev/null 2>&1 || true
    rm -f "$REG_FILE"
    log_success "Applied winhttp override successfully."
else
    log_info "Skipped automatic override."
fi

# ──────────────────────────────────────────────
# Fetch Latest Release
# ──────────────────────────────────────────────
echo ""
log_info "Fetching available LunacidAPClient releases..."
if ! command -v jq &>/dev/null; then
    log_error "jq is required but not installed."
    exit 1
fi

RELEASES_JSON=$(curl -s "$API_URL")
TAGS=($(echo "$RELEASES_JSON" | jq -r '.[] | "\(.tag_name) \(.published_at)"' | sort -rk2 | awk '{print $1}'))

FILTERED_TAGS=()
for tag in "${TAGS[@]}"; do
    v=${tag#v}
    IFS='.' read -r major minor patch <<< "$v"
    IFS='.' read -r min_maj min_min min_patch <<< "$MIN_VERSION"
    if (( major > min_maj || (major == min_maj && minor > min_min) || (major == min_maj && minor == min_min && patch >= min_patch) )); then
        FILTERED_TAGS+=("$tag")
    fi
done

if [ ${#FILTERED_TAGS[@]} -eq 0 ]; then
    log_error "No valid releases found (>= $MIN_VERSION)."
    exit 1
fi

log_info "Available versions:"
COLS=3
WIDTH=25
ROWS=$(( (${#FILTERED_TAGS[@]} + COLS - 1) / COLS ))

for r in $(seq 0 $((ROWS - 1))); do
    for c in $(seq 0 $((COLS - 1))); do
        i=$(( c*ROWS + r ))
        if [ $i -lt ${#FILTERED_TAGS[@]} ]; then
            TAG="${FILTERED_TAGS[$i]}"
            [ "$i" -eq 0 ] && TAG="${TAG} (latest)"
            printf "[%2d] %-*s" "$i" "$WIDTH" "$TAG"
        fi
    done
    echo ""
done
echo ""

read -rp "Select version [ENTER for latest]: " INDEX < /dev/tty
INDEX=${INDEX:-0}

if ! [[ "$INDEX" =~ ^[0-9]+$ ]] || [ "$INDEX" -ge "${#FILTERED_TAGS[@]}" ]; then
    log_error "Invalid selection."
    exit 1
fi

SELECTED_TAG="${FILTERED_TAGS[$INDEX]}"
log_info "Selected version: ${SELECTED_TAG}"

DOWNLOAD_URL=$(echo "$RELEASES_JSON" | jq -r ".[] | select(.tag_name==\"$SELECTED_TAG\") | .assets[] | select(.name | test(\"^Lunacid.*\\\\.zip$\")) | .browser_download_url")

if [ -z "$DOWNLOAD_URL" ]; then
    log_error "No valid LunacidAPClient zip found for ${SELECTED_TAG}."
    exit 1
fi

TMP_ZIP="/tmp/lunacid_${SELECTED_TAG}.zip"
curl -L -o "$TMP_ZIP" "$DOWNLOAD_URL"
unzip -o -q "$TMP_ZIP" -d "$GAME_DIR"
rm -f "$TMP_ZIP"

log_success "LunacidAPClient (${SELECTED_TAG}) installed successfully!"
