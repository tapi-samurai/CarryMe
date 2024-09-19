using UnityEngine;
using UniRx;
using UnityEngine.InputSystem;

// PlayerInput‚Ì“ü—Í‚ğŠÄ‹‘ÎÛ‚Ì•Ï”‚Æ‚µ‚Ä•Û‚·‚é

public class PlayerModel
{
    ReactiveProperty<Vector2> m_inputMove = new ReactiveProperty<Vector2>(Vector2.zero);
    ReactiveProperty<bool> m_inputJump = new ReactiveProperty<bool>(false);
    ReactiveProperty<bool> m_inputCarry1 = new ReactiveProperty<bool>(false);
    ReactiveProperty<bool> m_inputCarry2 = new ReactiveProperty<bool>(false);

    public IReadOnlyReactiveProperty<Vector2> InputMove { get { return m_inputMove; } }
    public IReadOnlyReactiveProperty<bool> InputJump { get { return m_inputJump; } }
    public IReadOnlyReactiveProperty<bool> InputCarry1 { get { return m_inputCarry1; } }
    public IReadOnlyReactiveProperty<bool> InputCarry2 { get { return m_inputCarry2; } }

    PlayerInput m_playerInput;

    public void SetPlayerInput(PlayerInput playerInput)
    {
        m_playerInput = playerInput;
    }

    // PlayerInput‚©‚ç‚Ì“ü—Í‚ğó‚¯æ‚éŠÖ”‚ğ“o˜^‚·‚é
    public void OnEntry()
    {
        m_playerInput.actions["Move"].performed += OnMove;
        m_playerInput.actions["Move"].canceled += OnMoveCanceld;
        m_playerInput.actions["Jump"].started += OnJump;
        m_playerInput.actions["Carry1"].started += OnCarryAndDrop1;
        m_playerInput.actions["Carry2"].started += OnCarryAndDrop2;
    }

    // OnEntry‚Å“o˜^‚³‚ê‚½ŠÖ”‚ğíœ‚·‚é
    public void OnDelete()
    {
        m_playerInput.actions["Move"].performed -= OnMove;
        m_playerInput.actions["Move"].canceled -= OnMoveCanceld;
        m_playerInput.actions["Jump"].started -= OnJump;
        m_playerInput.actions["Carry1"].started -= OnCarryAndDrop1;
        m_playerInput.actions["Carry2"].started -= OnCarryAndDrop2;
    }

    void OnMove(InputAction.CallbackContext callback)
    {
        m_inputMove.Value = callback.ReadValue<Vector2>();
    }

    void OnMoveCanceld(InputAction.CallbackContext callback)
    {
        m_inputMove.Value = Vector2.zero;
    }

    void OnJump(InputAction.CallbackContext callback)
    {
        m_inputJump.Value = !m_inputJump.Value;
    }

    void OnCarryAndDrop1(InputAction.CallbackContext callback)
    {
        m_inputCarry1.Value = !m_inputCarry1.Value;
    }

    void OnCarryAndDrop2(InputAction.CallbackContext callback)
    {
        m_inputCarry2.Value = !m_inputCarry2.Value;
    }
}
