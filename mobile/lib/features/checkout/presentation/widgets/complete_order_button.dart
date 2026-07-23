import 'package:flutter/material.dart';

class CompleteOrderButton extends StatelessWidget {
  const CompleteOrderButton({
    super.key,
    required this.isSubmitting,
    required this.onPressed,
    this.enabled = true,
  });
  final bool isSubmitting;
  final bool enabled;
  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) => FilledButton(
    onPressed: (isSubmitting || !enabled) ? null : onPressed,
    child: isSubmitting
        ? const SizedBox(
            height: 20,
            width: 20,
            child: CircularProgressIndicator(strokeWidth: 2),
          )
        : const Text('Siparişi Tamamla'),
  );
}
