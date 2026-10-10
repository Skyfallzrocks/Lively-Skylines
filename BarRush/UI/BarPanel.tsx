import React from "react";
import { bindValue, trigger, useValue } from "cs2/api";

const GROUP = "BarRush";

const visible$ = bindValue<boolean>(GROUP, "barPanelVisible", false);
const bars$ = bindValue<string>(GROUP, "barPanelBars", "");
const events$ = bindValue<string>(GROUP, "barPanelEvents", "");
const barCount$ = bindValue<number>(GROUP, "barPanelBarCount", 0);
const welcome$ = bindValue<boolean>(GROUP, "barPanelWelcome", false);

// Blue rounded-square tab button with a cocktail glass, in the style of the
// game's own icon buttons. Click it to open or close the Bars panel.
export const BarTabButton = () => {
    const visible = useValue(visible$);

    return (
        <button
            title="Bars"
            onClick={() => trigger(GROUP, "toggleBarPanel")}
            style={{
                // Pinned to the top-centre of the screen whatever container the game
                // mounts this in. To move it, change top / left / marginLeft
                // (marginLeft is minus half the button width).
                position: "fixed",
                top: "10rem",
                left: "50%",
                marginLeft: "-22rem",
                zIndex: 1000,
                width: "44rem",
                height: "44rem",
                padding: 0,
                border: "none",
                borderRadius: "8rem",
                background: visible ? "#1f5fa6" : "#2f86d6",
                cursor: "pointer",
                pointerEvents: "auto",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
            }}
        >
            <svg viewBox="0 0 24 24" width="28rem" height="28rem">
                <path d="M3 4h18l-8 9v6h4v2H7v-2h4v-6L3 4z" fill="white" />
            </svg>
        </button>
    );
};

const sectionTitle: React.CSSProperties = {
    fontSize: "15rem",
    fontWeight: "bold",
    margin: "10rem 0 6rem",
};

// Window with the events happening now and every bar in the city.
export const BarPanel = () => {
    const visible = useValue(visible$);
    const barsText = useValue(bars$);
    const eventsText = useValue(events$);
    const barCount = useValue(barCount$);
    const welcome = useValue(welcome$);

    if (!visible) {
        return null;
    }

    const bars = barsText ? barsText.split("\n") : [];
    const events = eventsText ? eventsText.split("\n") : [];

    return (
        <div
            style={{
                position: "fixed",
                top: "64rem",
                left: "50%",
                marginLeft: "-200rem",
                zIndex: 1000,
                width: "400rem",
                maxHeight: "70%",
                display: "flex",
                flexDirection: "column",
                padding: "14rem 18rem",
                borderRadius: "10rem",
                background: "rgba(20, 24, 40, 0.96)",
                color: "white",
                fontSize: "14rem",
                pointerEvents: "auto",
            }}
        >
            <div style={{ fontSize: "18rem", fontWeight: "bold" }}>
                {welcome ? "Thank you for installing Bar Rush!" : "Bars"}
            </div>
            {welcome && (
                <div style={{ opacity: 0.85, marginTop: "4rem" }}>
                    Here are the bars in your city and what is happening at them.
                    Use the blue tab button any time to open this again.
                </div>
            )}

            <div style={{ overflowY: "auto", flex: 1 }}>
                <div style={sectionTitle}>Happening now</div>
                {events.length === 0 ? (
                    <div style={{ opacity: 0.8 }}>No events right now.</div>
                ) : (
                    events.map((line, i) => (
                        <div key={i} style={{ padding: "3rem 0" }}>
                            {line}
                        </div>
                    ))
                )}

                <div style={sectionTitle}>All bars ({barCount})</div>
                {bars.length === 0 ? (
                    <div style={{ opacity: 0.8 }}>No bars in the city yet.</div>
                ) : (
                    bars.map((line, i) => (
                        <div key={i} style={{ padding: "3rem 0" }}>
                            {line}
                        </div>
                    ))
                )}
            </div>

            <div style={{ display: "flex", justifyContent: "flex-end", marginTop: "12rem" }}>
                <button
                    onClick={() => trigger(GROUP, "closeBarPanel")}
                    style={{
                        padding: "8rem 22rem",
                        borderRadius: "6rem",
                        border: "none",
                        background: "#3a7bd5",
                        color: "white",
                        fontSize: "14rem",
                        cursor: "pointer",
                    }}
                >
                    Exit
                </button>
            </div>
        </div>
    );
};
